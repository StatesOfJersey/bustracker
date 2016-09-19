using Newtonsoft.Json;
using System;

namespace Tracking_Common
{
    public class AssetLocationUpdate
    {
        public string DeviceId { get; set; }
        public string AssetType { get; set; }
        public string AssetRegistrationNumber { get; set; }
        public string ServiceNumber { get; set; }
        public string ServiceName { get; set; }
        public string ServiceOperator { get; set; }
        public DateTime? OriginalStartTime { get; set; }
        public DateTime TimeOfUpdate { get; set; }
        public string Direction { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int? Bearing { get; set; }


        public int SecondsAgo { get { return Convert.ToInt32((DateTime.Now - TimeOfUpdate).TotalSeconds); } set { } }

    }
}
