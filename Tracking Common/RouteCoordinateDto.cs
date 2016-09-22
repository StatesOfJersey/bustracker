namespace Tracking_Common
{
    public class RouteCoordinateDto
    {
        public decimal lat { get; set; }
        public decimal lon { get; set; }
        public bool isVital { get; set; }
        public bool occasional { get; set; }
        public string direction { get; set; }
        public string splitSection { get; set; }
    }
}
