using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Siri_Subscription;
using System.Xml.Linq;

namespace Tracking_Unit_Tests
{
    [TestClass]
    public class SiriIntegrationTests
    {
        [TestMethod]
        public void TestConvertServiceNumberToAssetType()
        {
            Communication comms = new Communication();
            PrivateObject obj = new PrivateObject(comms);
            Assert.AreEqual("Public Bus", obj.Invoke("ConvertServiceNumberToAssetType", "1", 30));
            Assert.AreEqual("Public Bus", obj.Invoke("ConvertServiceNumberToAssetType", "1a", 30));
            Assert.AreEqual("Public Bus", obj.Invoke("ConvertServiceNumberToAssetType", "1A", 30));
            Assert.AreEqual("Public Bus", obj.Invoke("ConvertServiceNumberToAssetType", "", 30));
            Assert.AreEqual("Public Bus", obj.Invoke("ConvertServiceNumberToAssetType", "30", 30));

            Assert.AreEqual("School Bus", obj.Invoke("ConvertServiceNumberToAssetType", "31", 30));
            Assert.AreEqual("School Bus", obj.Invoke("ConvertServiceNumberToAssetType", "899", 30));
            Assert.AreEqual("School Bus", obj.Invoke("ConvertServiceNumberToAssetType", "900", 30));
            Assert.AreEqual("School Bus", obj.Invoke("ConvertServiceNumberToAssetType", "901", 30));
            Assert.AreEqual("School Bus", obj.Invoke("ConvertServiceNumberToAssetType", "999", 30));
            Assert.AreEqual("School Bus", obj.Invoke("ConvertServiceNumberToAssetType", "900A", 30));




        }

        [TestMethod]
        public void TestConvertJourneyCodeToStartTime()
        {
            var now = DateTime.Now;
            Communication comms = new Communication();
            PrivateObject obj = new PrivateObject(comms);
            Assert.AreEqual(now.ToString("yyyy/MM/dd"), ((DateTime)obj.Invoke("ConvertJourneyCodeToStartTime", "1100")).ToString("yyyy/MM/dd"));
            Assert.AreEqual("11:00", ((DateTime)obj.Invoke("ConvertJourneyCodeToStartTime", "1100")).ToString("HH:mm"));
            Assert.AreEqual("09:57", ((DateTime)obj.Invoke("ConvertJourneyCodeToStartTime", "0957")).ToString("HH:mm"));
            Assert.AreEqual("23:01", ((DateTime)obj.Invoke("ConvertJourneyCodeToStartTime", "2301")).ToString("HH:mm"));

            //ConvertJourneyCodeToStartTime
        }

        [TestMethod]
        public void TestConvertToSiriMessageToAssetLocationUpdates_DifferentBuses()
        {
            string updateMessage = @"<Siri xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.kizoom.com/standards/siri/schema/1.3/siri.xsd"" version=""1.3"" xmlns=""http://www.siri.org.uk/siri"">
  <ServiceDelivery>
    <ResponseTimestamp>2015-11-10T11:16:08+00:00</ResponseTimestamp>
              <ProducerRef>TKTR01Live</ProducerRef>
              <Status>true</Status>
              <MoreData>false</MoreData>
              <VehicleMonitoringDelivery version = ""1.3"">
                 <ResponseTimestamp>2015-11-10T11:16:08+00:00</ResponseTimestamp>
<SubscriberRef>DATAJEAPI</SubscriberRef>
<SubscriptionRef>01000459</SubscriptionRef>
<Status>true</Status>
<VehicleActivity>
<RecordedAtTime>2015-11-10T11:13:20+00:00</RecordedAtTime>
<ValidUntilTime>2015-11-10T11:13:20+00:00</ValidUntilTime>
<MonitoredVehicleJourney>
<LineRef>23</LineRef>
<DirectionRef>outbound</DirectionRef>
<FramedVehicleJourneyRef>
<DataFrameRef>2015-11-10</DataFrameRef>
<DatedVehicleJourneyRef>23_20151110_10_56</DatedVehicleJourneyRef>
</FramedVehicleJourneyRef>
<PublishedLineName>23</PublishedLineName>
<OperatorRef>Jersey</OperatorRef>
<VehicleLocation>
<Longitude>-2.097275</Longitude>
<Latitude>49.192932</Latitude>
</VehicleLocation>
<Bearing>38</Bearing>
<BlockRef>1139</BlockRef>
<VehicleRef>316</VehicleRef>
</MonitoredVehicleJourney>
<Extensions>
<VehicleJourney>
<Operational>
<TicketMachine>
<TicketMachineServiceCode>23O</TicketMachineServiceCode>
<JourneyCode>1100</JourneyCode>
</TicketMachine>
</Operational>
<VehicleUniqueId>00016</VehicleUniqueId>
</VehicleJourney>
</Extensions>
</VehicleActivity>
<VehicleActivity>
<RecordedAtTime>2015-11-10T11:14:19+00:00</RecordedAtTime>
<ValidUntilTime>2015-11-10T11:14:19+00:00</ValidUntilTime>
        <MonitoredVehicleJourney>
            <LineRef>9</LineRef>
            <DirectionRef>outbound</DirectionRef>
            <FramedVehicleJourneyRef>
            <DataFrameRef>2015-11-10</DataFrameRef>
            <DatedVehicleJourneyRef>23_20151110_10_56</DatedVehicleJourneyRef>
            </FramedVehicleJourneyRef>
            <PublishedLineName>9</PublishedLineName>
            <OperatorRef>Jersey</OperatorRef>
            <VehicleLocation>
                <Longitude>-2.091063</Longitude>
                <Latitude>49.194402</Latitude>
            </VehicleLocation>
            <Bearing>220</Bearing>
            <BlockRef>1140</BlockRef>
            <VehicleRef>315</VehicleRef>
            </MonitoredVehicleJourney>
            <Extensions>
            <VehicleJourney>
                <Operational>
                <TicketMachine>
                    <TicketMachineServiceCode>23O</TicketMachineServiceCode>
                        <JourneyCode>1100</JourneyCode>
                    </TicketMachine>
                </Operational>
                    <VehicleUniqueId>00017</VehicleUniqueId>
                </VehicleJourney>
            </Extensions>
            </VehicleActivity>
   </VehicleMonitoringDelivery>
  </ServiceDelivery>
</Siri>";
            Communication comms = new Communication();
            var output = comms.ConvertToSiriMessageToAssetLocationUpdates(XDocument.Parse(updateMessage), 28, "01000459");
            Assert.AreEqual(2, output.Count);
            Assert.AreEqual("bus-23-1100-316", output[0].DeviceId);
            Assert.AreEqual("bus-9-1100-315", output[1].DeviceId);
        }

        [TestMethod]
        public void TestSiriMessageFromUnknownOriginIsIgnored()
        {
            string updateMessage = @"<Siri xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.kizoom.com/standards/siri/schema/1.3/siri.xsd"" version=""1.3"" xmlns=""http://www.siri.org.uk/siri"">
  <ServiceDelivery>
    <ResponseTimestamp>2015-11-10T11:16:08+00:00</ResponseTimestamp>
              <ProducerRef>TKTR01Live</ProducerRef>
              <Status>true</Status>
              <MoreData>false</MoreData>
              <VehicleMonitoringDelivery version = ""1.3"">
                 <ResponseTimestamp>2015-11-10T11:16:08+00:00</ResponseTimestamp>
<SubscriberRef>DATAJEAPI</SubscriberRef>
<SubscriptionRef>01000459</SubscriptionRef>
<Status>true</Status>
<VehicleActivity>
<RecordedAtTime>2015-11-10T11:13:20+00:00</RecordedAtTime>
<ValidUntilTime>2015-11-10T11:13:20+00:00</ValidUntilTime>
<MonitoredVehicleJourney>
<LineRef>23</LineRef>
<DirectionRef>outbound</DirectionRef>
<FramedVehicleJourneyRef>
<DataFrameRef>2015-11-10</DataFrameRef>
<DatedVehicleJourneyRef>23_20151110_10_56</DatedVehicleJourneyRef>
</FramedVehicleJourneyRef>
<PublishedLineName>23</PublishedLineName>
<OperatorRef>Jersey</OperatorRef>
<VehicleLocation>
<Longitude>-2.097275</Longitude>
<Latitude>49.192932</Latitude>
</VehicleLocation>
<Bearing>38</Bearing>
<BlockRef>1139</BlockRef>
<VehicleRef>316</VehicleRef>
</MonitoredVehicleJourney>
<Extensions>
<VehicleJourney>
<Operational>
<TicketMachine>
<TicketMachineServiceCode>23O</TicketMachineServiceCode>
<JourneyCode>1100</JourneyCode>
</TicketMachine>
</Operational>
<VehicleUniqueId>00016</VehicleUniqueId>
</VehicleJourney>
</Extensions>
</VehicleActivity>
<VehicleActivity>
<RecordedAtTime>2015-11-10T11:14:19+00:00</RecordedAtTime>
<ValidUntilTime>2015-11-10T11:14:19+00:00</ValidUntilTime>
        <MonitoredVehicleJourney>
            <LineRef>9</LineRef>
            <DirectionRef>outbound</DirectionRef>
            <FramedVehicleJourneyRef>
            <DataFrameRef>2015-11-10</DataFrameRef>
            <DatedVehicleJourneyRef>23_20151110_10_56</DatedVehicleJourneyRef>
            </FramedVehicleJourneyRef>
            <PublishedLineName>9</PublishedLineName>
            <OperatorRef>Jersey</OperatorRef>
            <VehicleLocation>
                <Longitude>-2.091063</Longitude>
                <Latitude>49.194402</Latitude>
            </VehicleLocation>
            <Bearing>220</Bearing>
            <BlockRef>1140</BlockRef>
            <VehicleRef>315</VehicleRef>
            </MonitoredVehicleJourney>
            <Extensions>
            <VehicleJourney>
                <Operational>
                <TicketMachine>
                    <TicketMachineServiceCode>23O</TicketMachineServiceCode>
                        <JourneyCode>1100</JourneyCode>
                    </TicketMachine>
                </Operational>
                    <VehicleUniqueId>00017</VehicleUniqueId>
                </VehicleJourney>
            </Extensions>
            </VehicleActivity>
   </VehicleMonitoringDelivery>
  </ServiceDelivery>
</Siri>";
            Communication comms = new Communication();
            var output = comms.ConvertToSiriMessageToAssetLocationUpdates(XDocument.Parse(updateMessage), 28, "madeUp");
            Assert.AreEqual(0, output.Count);
        }

        [TestMethod]
        public void TestConvertToSiriMessageToAssetLocationUpdates()
        {
            string updateMessage = @"<Siri xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.kizoom.com/standards/siri/schema/1.3/siri.xsd"" version=""1.3"" xmlns=""http://www.siri.org.uk/siri"">
  <ServiceDelivery>
    <ResponseTimestamp>2015-11-10T11:16:08+00:00</ResponseTimestamp>
              <ProducerRef>TKTR01Live</ProducerRef>
              <Status>true</Status>
              <MoreData>false</MoreData>
              <VehicleMonitoringDelivery version = ""1.3"">
                 <ResponseTimestamp>2015-11-10T11:16:08+00:00</ResponseTimestamp>
<SubscriberRef>DATAJEAPI</SubscriberRef>
<SubscriptionRef>01000459</SubscriptionRef>
<Status>true</Status>
<VehicleActivity>
<RecordedAtTime>2015-11-10T11:13:20+00:00</RecordedAtTime>
<ValidUntilTime>2015-11-10T11:13:20+00:00</ValidUntilTime>
<MonitoredVehicleJourney>
<LineRef>23</LineRef>
<DirectionRef>outbound</DirectionRef>
<FramedVehicleJourneyRef>
<DataFrameRef>2015-11-10</DataFrameRef>
<DatedVehicleJourneyRef>23_20151110_10_56</DatedVehicleJourneyRef>
</FramedVehicleJourneyRef>
<PublishedLineName>23</PublishedLineName>
<OperatorRef>Jersey</OperatorRef>
<VehicleLocation>
<Longitude>-2.097275</Longitude>
<Latitude>49.192932</Latitude>
</VehicleLocation>
<Bearing>38</Bearing>
<BlockRef>1139</BlockRef>
<VehicleRef>316</VehicleRef>
</MonitoredVehicleJourney>
<Extensions>
<VehicleJourney>
<Operational>
<TicketMachine>
<TicketMachineServiceCode>23O</TicketMachineServiceCode>
<JourneyCode>1100</JourneyCode>
</TicketMachine>
</Operational>
<VehicleUniqueId>00016</VehicleUniqueId>
</VehicleJourney>
</Extensions>
</VehicleActivity>
<VehicleActivity>
<RecordedAtTime>2015-11-10T11:14:19+00:00</RecordedAtTime>
<ValidUntilTime>2015-11-10T11:14:19+00:00</ValidUntilTime>
        <MonitoredVehicleJourney>
            <LineRef>23</LineRef>
            <DirectionRef>outbound</DirectionRef>
            <FramedVehicleJourneyRef>
            <DataFrameRef>2015-11-10</DataFrameRef>
            <DatedVehicleJourneyRef>23_20151110_10_56</DatedVehicleJourneyRef>
            </FramedVehicleJourneyRef>
            <PublishedLineName>23</PublishedLineName>
            <OperatorRef>Jersey</OperatorRef>
            <VehicleLocation>
                <Longitude>-2.091066</Longitude>
                <Latitude>49.194404</Latitude>
            </VehicleLocation>
            <Bearing>59</Bearing>
            <BlockRef>1139</BlockRef>
            <VehicleRef>316</VehicleRef>
            </MonitoredVehicleJourney>
            <Extensions>
            <VehicleJourney>
                <Operational>
                <TicketMachine>
                    <TicketMachineServiceCode>23O</TicketMachineServiceCode>
                        <JourneyCode>1100</JourneyCode>
                    </TicketMachine>
                </Operational>
                    <VehicleUniqueId>00016</VehicleUniqueId>
                </VehicleJourney>
            </Extensions>
            </VehicleActivity>
   </VehicleMonitoringDelivery>
  </ServiceDelivery>
</Siri>";

            Communication comms = new Communication();
            var output = comms.ConvertToSiriMessageToAssetLocationUpdates(XDocument.Parse(updateMessage), 28, "01000459");
            Assert.AreEqual(2, output.Count);
            Assert.AreEqual("bus-23-1100-316", output[0].DeviceId);
            ;
        }

    }
}
