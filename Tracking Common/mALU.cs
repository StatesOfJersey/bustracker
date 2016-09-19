using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tracking_Common
{
    [DataContract]
    public class mALU 
    {
        private AssetLocationUpdate _alu;
        public mALU(AssetLocationUpdate update)
        {
            _alu = update;
        }

        public mALU()
        { }
        
        [DataMember]
        public string bus { get { return _alu.DeviceId; } set { } }
        [DataMember]
        public string line { get { return _alu.ServiceNumber; } set { } }
        [DataMember]
        public string cat { get { return _alu.AssetType; }  set { } }
        [DataMember]
        public decimal lat { get { return _alu.Latitude; }  set { } }
        [DataMember]
        public decimal lon { get { return _alu.Longitude; } set { } }
        [DataMember]
        public int? bearing { get { return _alu.Bearing; } set { } }
        [DataMember]
        public string direction { get { return _alu.Direction; } set { } }
        [DataMember]
        public string time { get { return _alu.TimeOfUpdate.ToString("HH:mm:ss"); } set { } }
        [DataMember]
        public int age { get { return _alu.SecondsAgo; } set { } }


    }
}
