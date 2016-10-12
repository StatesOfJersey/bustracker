using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Tracking_Common
{
    public class BusETA
    {
        public int StopNumber { get; set; }
        public string ServiceNumber { get; set; }
        public string Destination { get; set; }
        public DateTime ETA { get; set; }
        public bool IsTracked { get; set; } 

        public static List<BusETA> ConvertVixXhtmlToBusETAs(string vixXhtml, DateTime referenceTime, int StopNumber)
        {
            if (vixXhtml.Contains("No departures found"))
            {
                return new List<BusETA>();
            }

            int startOfTable = vixXhtml.IndexOf("<table class=\"webDisplayTable\"");
            int endOfTable = vixXhtml.IndexOf("</table>", startOfTable);



            var innerTable = vixXhtml.Substring(startOfTable, endOfTable - startOfTable + "</table>".Length).Replace("&nbsp;", "");
            var result = XDocument.Parse(innerTable);

            var tableRows = result.XPathSelectElements("//tr").ToList();

            List<BusETA> busETAs = new List<BusETA>();
            for (int i = 0; i < tableRows.Count; i++)
            {
                var columns = XElement.Parse(tableRows[i].ToString(SaveOptions.DisableFormatting)).XPathSelectElements("//td").ToList();
                if (columns.Count() > 0)
                {
                    var busETA = new BusETA();

                    busETA.ServiceNumber = columns[0].Value;
                    busETA.Destination = columns[2].Value;
                    try
                    {
                        if (columns[4].Value.Contains("Due"))
                        {
                            busETA.ETA = referenceTime;
                            busETA.IsTracked = true;
                        }
                        else if (columns[4].Value.Contains(":"))
                        {

                            busETA.ETA =
                                DateTime.Parse(referenceTime.ToString("yyyy/MM/dd ") + columns[4].Value + ((TimeZoneInfo.Local.IsDaylightSavingTime(referenceTime)) ? "+01:00" : ""));  
                            busETA.IsTracked = false;
                        }
                        else
                        {
                            busETA.ETA = referenceTime.AddMinutes(int.Parse(Regex.Replace(columns[4].Value, "[^\\d]", "")));
                            busETA.IsTracked = true;
                        }

                    }
                    catch (Exception ex)
                    {
                        busETA.ETA = referenceTime;
                    }
                    busETA.StopNumber = StopNumber;
                    if (busETA.ETA >= referenceTime)
                    {
                        //Only show buses that are coming, not ones that have gone
                        busETAs.Add(busETA);
                    }
                }

            }
            return busETAs;
        }

    }
}
