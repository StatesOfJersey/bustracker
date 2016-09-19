namespace Tracking_Common
{
    public class RouteCoordinateDto
    {
        public decimal lat { get; set; }
        public decimal lon { get; set; }
        public bool isVital { get; set; }
        public bool occasional { get; set; }
        public int direction { get; set; }
    }
}
