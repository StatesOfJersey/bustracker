using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracking_Common
{
    public class BusStop
    {
        public int StopNumber { get; set; }
        public string StopName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int DistanceAwayInMetres { get; set; }
    }
}
