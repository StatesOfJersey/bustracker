using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Tracking_Common;

namespace Siri_Subscription
{
    public class Communication : IDisposable
    {
        // set up new service 
        public Communication()
        {

        }

        public Tuple<bool, XDocument> SubscribeToService(string requestorRef, string replyAddress, string subscriptionIdentifier, string updateInterval, string urlForTicketer, string ticketerLogin, string ticketerPassword)
        {
            return CallWebService(
string.Format(@"<Siri xmlns=""http://www.siri.org.uk/siri"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.kizoom.com/standards/siri/schema/1.3/siri.xsd"" version=""1.3"">
  <SubscriptionRequest>
    <RequestTimestamp>{0}T{1}+00:00</RequestTimestamp>
    <RequestorRef>{4}</RequestorRef>
    <ConsumerAddress>{5}</ConsumerAddress>
    <SubscriptionContext>
      <HeartbeatInterval>PT60S</HeartbeatInterval>
    </SubscriptionContext>
    <VehicleMonitoringSubscriptionRequest>
      <SubscriptionIdentifier>{6}</SubscriptionIdentifier>
      <InitialTerminationTime>{2}T{3}+00:00</InitialTerminationTime>
      <VehicleMonitoringRequest version=""1.3"">
        <RequestTimestamp>{0}T{1}+00:00</RequestTimestamp>
        <VehicleMonitoringDetailLevel>normal</VehicleMonitoringDetailLevel>
      </VehicleMonitoringRequest>
      <IncrementalUpdates>true</IncrementalUpdates>
      <UpdateInterval>{7}</UpdateInterval>
    </VehicleMonitoringSubscriptionRequest>
  </SubscriptionRequest>
</Siri>", DateTime.UtcNow.ToString("yyyy-MM-dd"),
          DateTime.UtcNow.ToString("HH:mm:ss"),
          DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-dd"),
          DateTime.UtcNow.ToString("HH:mm:ss"),
          requestorRef,
          replyAddress,
          subscriptionIdentifier,
          updateInterval), urlForTicketer, ticketerLogin, ticketerPassword);
        }

        public Tuple<bool, XDocument> AcknowledgeHeartbeat(string urlForTicketer, string ticketerLogin, string ticketerPassword)
        {
            return CallWebService(
            HeartbeatResponse(), urlForTicketer, ticketerLogin, ticketerPassword);
        }

        public string HeartbeatResponse()
        {
            return string.Format(@"<Siri xmlns=""http://www.siri.org.uk/siri"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.kizoom.com/standards/siri/schema/1.3/siri.xsd"" version=""1.3"">
  <CheckStatusResponse>
    <RequestTimestamp>{0}T{1}+00:00</RequestTimestamp>
    <Status>true</Status>
  </CheckStatusResponse>
</Siri>", DateTime.UtcNow.ToString("yyyy-MM-dd"), DateTime.UtcNow.ToString("HH:mm:ss"));
        }


        public List<AssetLocationUpdate> ConvertToSiriMessageToAssetLocationUpdates(XDocument siriMessage, int cutOffForSchoolBusNumbers, string subscriptionRef)
        {
            siriMessage.StripSiriNamespace();
            List<AssetLocationUpdate> locationUpdates = new List<AssetLocationUpdate>();

            if (siriMessage.XPathSelectElement("/Siri/ServiceDelivery/VehicleMonitoringDelivery/SubscriptionRef").Value != subscriptionRef)
            {
                //This message did not include our secret subscriptionRef so ignore it by quietly just not returning any updates
                return new List<AssetLocationUpdate>();
            }

            foreach (XElement individualUpdateParent in siriMessage.XPathSelectElements("/Siri/ServiceDelivery/VehicleMonitoringDelivery/VehicleActivity"))
            {
                XElement individualUpdate = XElement.Parse(individualUpdateParent.ToString(SaveOptions.DisableFormatting));
                AssetLocationUpdate locationUpdate = new AssetLocationUpdate();

                locationUpdate.DeviceId = string.Format("bus-{0}-{1}-{2}", individualUpdate.XPathSelectElement("//LineRef").Value, individualUpdate.XPathSelectElement("//JourneyCode").Value, individualUpdate.XPathSelectElement("//VehicleRef").Value);
                locationUpdate.AssetType = ConvertServiceNumberToAssetType(individualUpdate.XPathSelectElement("//LineRef").Value, cutOffForSchoolBusNumbers);
                locationUpdate.AssetRegistrationNumber = individualUpdate.XPathSelectElement("//VehicleRef").Value;
                locationUpdate.ServiceNumber = individualUpdate.XPathSelectElement("//LineRef").Value;
                locationUpdate.ServiceName = individualUpdate.XPathSelectElement("//PublishedLineName").Value;
                locationUpdate.ServiceOperator = "LibertyBus";
                locationUpdate.OriginalStartTime = ConvertJourneyCodeToStartTime(individualUpdate.XPathSelectElement("//JourneyCode").Value);
                locationUpdate.TimeOfUpdate = DateTime.Parse(individualUpdate.XPathSelectElement("//RecordedAtTime").Value, null);
                locationUpdate.Direction = individualUpdate.XPathSelectElement("//DirectionRef").Value;
                locationUpdate.Latitude = decimal.Parse(individualUpdate.XPathSelectElement("//Latitude").Value);
                locationUpdate.Longitude = decimal.Parse(individualUpdate.XPathSelectElement("//Longitude").Value);
                locationUpdate.Bearing = int.Parse(individualUpdate.XPathSelectElement("//Bearing").Value);
                locationUpdates.Add(locationUpdate);
            };
            return locationUpdates;
        }

        private string ConvertServiceNumberToAssetType(string serviceNumber, int cutOffForSchoolBusNumbers)
        {
            try
            {
                var serviceNumerics = Regex.Match(serviceNumber, @"\d+").Value;
                return int.Parse(serviceNumerics) > cutOffForSchoolBusNumbers ? "School Bus" : "Public Bus";
            }
            catch (Exception)
            { return "Public Bus"; }
        }
        private DateTime ConvertJourneyCodeToStartTime(string journeyCode)
        {
            var provider = new CultureInfo("en-GB");
            return DateTime.ParseExact(DateTime.Now.ToString("yyyy/MM/dd ") + journeyCode, "yyyy/MM/dd HHmm", provider);
        }

        private Tuple<bool, XDocument> CallWebService(string xmlPayload, string urlForTicketer, string ticketerLogin, string ticketerPassword)
        {
            string result = "";
            string URL_ADDRESS = urlForTicketer;

            // Create the web request
            HttpWebRequest request = WebRequest.Create(new Uri(URL_ADDRESS)) as HttpWebRequest;

            // Set type to POST
            request.Method = "POST";
            request.ContentType = "application/xml";
            request.Credentials = new NetworkCredential(ticketerLogin, ticketerPassword);

            StringBuilder data = new StringBuilder();
            data.Append(xmlPayload);
            byte[] byteData = Encoding.UTF8.GetBytes(data.ToString());
            request.ContentLength = byteData.Length;

            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            XDocument xmlResult;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
                reader.Close();
            }

            bool success = false;
            try
            {
                xmlResult = XDocument.Parse(result);
                xmlResult.StripSiriNamespace();
                success = bool.Parse(xmlResult.XPathSelectElement("//ResponseStatus/Status").Value);
            }
            catch
            {
                //ignore we are going to just report no success
                xmlResult = XDocument.Parse("<xmlResponse><![CDATA[ could not parse the following : '" + result + "']]></xmlResponse>");
            }

            return new Tuple<bool, XDocument>(
                success,
                xmlResult);
        }

        public void Dispose()
        {

        }
    }
}
