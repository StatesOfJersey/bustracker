using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using Tracking_Common;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Newtonsoft.Json;

namespace UAT_Tracking_Subscription_WebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([ServiceBusTrigger("buses", "uat-subscriber")] BrokeredMessage message, TextWriter log)
        {
            //Log
            log.WriteLine("Message picked up from buses topic : " + message.MessageId);

            var body = message.GetBody<string>();

            SendLiveLocationToSqlAzure(JsonConvert.DeserializeObject(body));
        }

        private static  void SendLiveLocationToSqlAzure(dynamic update)
        {
            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["SQLAzure.ConnectionString"]))
            {
                var command = new SqlCommand("addLiveLocaton", connection) { CommandType = CommandType.StoredProcedure };

                command.Parameters.Add("@deviceId", SqlDbType.NVarChar, 50).Value = update.From; // update.DeviceId;
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

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }

    }
}
