using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracking_Common
{
    public class BusRoute
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Colour { get; set; }
        public string ColourInverse { get; set; }
        public IList<RouteCoordinateDto> RouteCoordinates { get; set; }
        public bool Active { get; set; }
    }
}
