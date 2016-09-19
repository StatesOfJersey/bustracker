using System;
using System.Collections.Generic;
using System.Linq;

namespace Tracking_Common
{
    public class BusStopDto
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string VixName { get; set; }
        public int Easting { get; set; }
        public int Northing { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool MarkedOnRoad { get; set; }
        public string MarkedOnRoadString => MarkedOnRoad ? "Yes" : "No";
        public bool InUse { get; set; }
        public string InUseString => InUse ? "Yes" : "No";

        public static IList<BusStopDto> ConvertFromCsvLines(IList<IList<string>> lines, bool inUseOnly)
        {
            //throw format exception if not correct file format
            //List line number and the formattin error
            if (BusStopValidator.ValidatateCsv(lines).Any())
            {
                throw new FormatException("Bus stop csv contents failed validation tests.");
            }
            var qry = lines.Skip(1).Select(l => new BusStopDto
            {
                Code = ToInt(l[0]),
                Name = l[1],
                VixName = l[2],
                Easting = ToInt(l[3]),
                Northing = ToInt(l[4]),
                Latitude = ToDecimal(l[5]),
                Longitude = ToDecimal(l[6]),
                MarkedOnRoad = l[7] == "y",
                InUse = l[8] == "y"
            }).AsQueryable();

            if (inUseOnly) qry = qry.Where(t => t.InUse);
            return qry.ToList();
        }

        private static decimal ToDecimal(string doubleString)
        {
            decimal dbl;
            decimal.TryParse(doubleString, out dbl);
            return dbl;
        }

        private static int ToInt(string stop)
        {
            int stopNo;
            int.TryParse(stop, out stopNo);
            return stopNo;
        }
    }
}