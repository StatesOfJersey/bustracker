using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Newtonsoft.Json;
using Siri_Subscription;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;
using Tracking_Common;
using StackExchange.Redis;

namespace Tracking_Receiver.Controllers
{
    public class ValuesController : ApiController
    {
        public static DateTime? SiriSubscriptionDate;

        private static Lazy<ConnectionMultiplexer> lazyRedisConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(CloudConfigurationManager.GetSetting("Redis.Cache.Key"));
        });

        public static ConnectionMultiplexer RedisConnection
        {
            get
            {
                return lazyRedisConnection.Value;
            }
        }
        // POST api/values
        public HttpResponseMessage Post(HttpRequestMessage request)
        {
            var reader = new StreamReader(request.Content.ReadAsStreamAsync().Result);
            string xmlFromSiri = reader.ReadToEnd();
            var xmlDocument = XDocument.Parse(xmlFromSiri);

            using (Communication comms = new Communication())
            {

                if (xmlFromSiri.IndexOf("CheckStatusRequest") > 0)
                {
                    //Heartbeat request, we need to respond to it to confirm our sevice is up and receiving messages
                    string heartbeatResponse = comms.HeartbeatResponse();
                    HttpResponseMessage httpResponse = new HttpResponseMessage();
                    httpResponse.Content = new StringContent(heartbeatResponse);
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(heartbeatResponse, Encoding.UTF8, "application/xml")
                    };
                }
                else if (xmlFromSiri.IndexOf("HeartbeatNotification") > 0)
                {
                    //do nothing it's a heartbeat from Siri
                }
                else
                {
                    //Bus location update so convert it to an AssetLocationUpdate and send on

                    bool messageBusEnabled = bool.Parse(CloudConfigurationManager.GetSetting("MessageBus.Enabled"));
                    TopicClient senderTopicClient = null;
                    if (messageBusEnabled)
                        senderTopicClient = TopicClient.CreateFromConnectionString(CloudConfigurationManager.GetSetting("MessageBus.ConnectionString"), CloudConfigurationManager.GetSetting("MessageBus.Topic"));

                    List<AssetLocationUpdate> assetLocationUpdates = comms.ConvertToSiriMessageToAssetLocationUpdates(xmlDocument, int.Parse(CloudConfigurationManager.GetSetting("CutOffForSchoolBusNumbers")), CloudConfigurationManager.GetSetting("Ticketer.SubscriptionIdentifier"));
                    foreach (var update in assetLocationUpdates)
                    {
                        sendLiveLocationToSqlAzure(update);

                        var eventData = new
                        {
                            From = update.DeviceId,
                            AssetType = update.AssetType,
                            AssetRegistrationNumber = update.AssetRegistrationNumber,
                            ServiceNumber = update.ServiceNumber,
                            ServiceName = update.ServiceName,
                            ServiceOperator = update.ServiceOperator,
                            OriginalStartTime = update.OriginalStartTime,
                            TimeOfUpdate = update.TimeOfUpdate,
                            Direction = update.Direction,
                            Latitude = update.Latitude,
                            Longitude = update.Longitude,
                            Bearing = update.Bearing
                        };
                        try
                        {
                            var busData = JsonConvert.SerializeObject(eventData);
                            if (senderTopicClient != null)
                                senderTopicClient.Send(new BrokeredMessage(busData));
                        }
                        catch (Exception ex)
                        { //no nothing
                        }

                    }
                    if (senderTopicClient != null)
                        senderTopicClient.Close();


                    try
                    {
                        IDatabase cache = RedisConnection.GetDatabase();
                        foreach (var update in assetLocationUpdates)
                        {
                            cache.StringSet("buses:bus:" + update.AssetRegistrationNumber, JsonConvert.SerializeObject(update), expiry: TimeSpan.FromMinutes(20));
                        }
                        cache.StringSet("bus:lastUpdate", DateTime.UtcNow.ToString());
                    }
                    catch (Exception ex)
                    {
                        //do nothing
                    }
                }
            }
            CheckAndSubscribeToSiriIfNecessary();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        internal static void CheckAndSubscribeToSiriIfNecessary()
        {
            if (SiriSubscriptionDate == null || SiriSubscriptionDate.Value < DateTime.UtcNow.AddDays(-1))
            {
                using (Communication comms = new Communication())
                {
                    var result = comms.SubscribeToService(
                        CloudConfigurationManager.GetSetting("Ticketer.RequestorRef"),
                        CloudConfigurationManager.GetSetting("Ticketer.ReplyAddress"),
                        CloudConfigurationManager.GetSetting("Ticketer.SubscriptionIdentifier"),
                        CloudConfigurationManager.GetSetting("Ticketer.UpdateInterval"),
                        CloudConfigurationManager.GetSetting("Ticketer.Url"),
                        CloudConfigurationManager.GetSetting("Ticketer.Login"),
                        CloudConfigurationManager.GetSetting("Ticketer.Password"));
                    if (result.Item1)//success
                    {
                        sendSubscriptionInformationToSqlAzure(result);
                        SiriSubscriptionDate = DateTime.UtcNow;
                    }
                }
            }
        }

        private void sendLiveLocationToSqlAzure(AssetLocationUpdate update)
        {
            SqlConnection connection = new SqlConnection(CloudConfigurationManager.GetSetting("SQLAzure.ConnectionString"));
            SqlCommand command = new SqlCommand("addLiveLocaton", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@deviceId", SqlDbType.NVarChar, 50).Value = update.DeviceId;
            command.Parameters.Add("@AssetType", SqlDbType.NVarChar, 50).Value = update.AssetType;
            command.Parameters.Add("@AssetRegistrationNumber", SqlDbType.NVarChar, 50).Value = update.AssetRegistrationNumber;
            command.Parameters.Add("@ServiceNumber", SqlDbType.NVarChar, 50).Value = update.ServiceNumber;
            command.Parameters.Add("@ServiceName", SqlDbType.NVarChar, 50).Value = update.ServiceName;
            command.Parameters.Add("@ServiceOperator", SqlDbType.NVarChar, 50).Value = update.ServiceOperator;
            command.Parameters.Add("@OriginalStartTime", SqlDbType.DateTime).Value = update.OriginalStartTime;
            command.Parameters.Add("@TimeOfUpdate", SqlDbType.DateTime).Value = update.TimeOfUpdate;
            command.Parameters.Add("@Direction", SqlDbType.NVarChar, 50).Value = update.Direction;
            command.Parameters.Add("@Latitude", SqlDbType.Decimal).Value = update.Latitude;
            command.Parameters.Add("@Longitude", SqlDbType.Decimal).Value = update.Longitude;
            command.Parameters.Add("@Bearing", SqlDbType.NVarChar, 50).Value = update.Bearing;

            // Execute command.
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        private static void sendSubscriptionInformationToSqlAzure(Tuple<bool, XDocument> update)
        {
            SqlConnection connection = new SqlConnection(CloudConfigurationManager.GetSetting("SQLAzure.ConnectionString"));
            SqlCommand command = new SqlCommand("addSubscriptionInformation", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@subscriptionStatus", SqlDbType.Bit).Value = update.Item1;
            command.Parameters.Add("@subscriptionXml", SqlDbType.NVarChar, -1).Value = update.Item2.ToString();

            // Execute command.
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
