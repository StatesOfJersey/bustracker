using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tracking_Common;

namespace Tracking_Unit_Tests
{
    [TestClass]
    public class StopUploadValidationTests
    {
        [TestMethod]
        public void ParseCorrectData_ReturnsNoValidationErrors_Succeeds()
        {  
            var headers =  new List<string>
                {
                    "Code",
                    "Name",
                    "Vix name",
                    "Easting",
                    "Northing",
                    "Latitude",
                    "Longitude",
                    "Marked on road?",
                    "In use Jan 2016?"
                };

            var rowOne = new List<string>
                {
                    "10001",
                    "Stop A",
                    "Stop A",
                    "1000",
                    "1000",
                    "1000.001",
                    "500.001",
                    "y",
                    "y"
                };

            var testData = new List<IList<string>> {headers,rowOne};
            var result = BusStopValidator.ValidatateCsv(testData);

            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void ParseIncorrectColumnCount_ReturnsInvalidColumns_Succeeds()
        {
            var headers = new List<string>
                {
                    "Code",
                    "Name",
                    "Vix name",
                    "Easting",
                    "Northing",
                    "Latitude",
                    "Longitude",
                    "Marked on road?"
                };

            var rowOne = new List<string>
                {
                    "10001",
                    "Stop A",
                    "Stop A",
                    "1000",
                    "1000",
                    "1000.001",
                    "500.001",
                    "y",
                    "y"
                };

            var testData = new List<IList<string>> { headers, rowOne };
            var result = BusStopValidator.ValidatateCsv(testData);

            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        public void ParseIncorrectData_ReturnsValidationMessages_Succeeds()
        {
            var headers = new List<string>
                {
                    "Code",
                    "Name",
                    "Vix name",
                    "Easting",
                    "Northing",
                    "Latitude",
                    "Longitude",
                    "Marked on road?",
                    "In use Jan 2016?"
                };

            var rowOne = new List<string>
                {
                    "SAMMMM",
                    null,
                    null,
                    "ddd",
                    "ddd",
                    "ddd",
                    "ewefw",
                    "x",
                    "x"
                };

            var testData = new List<IList<string>> { headers, rowOne };
            var result = BusStopValidator.ValidatateCsv(testData);

            Assert.AreEqual(result.Count, 9);
        }

        [TestMethod]
        public void ParseNoRows_ReturnsValidationMessages_Succeeds()
        {
            var headers = new List<string>
                {
                    "Code",
                    "Name",
                    "Vix name",
                    "Easting",
                    "Northing",
                    "Latitude",
                    "Longitude",
                    "Marked on road?",
                    "In use Jan 2016?"
                };

            var testData = new List<IList<string>> { headers};
            var result = BusStopValidator.ValidatateCsv(testData);

            Assert.AreEqual(result.Count, 1);
        }


        [TestMethod]
        public void ParseMissingCells_ReturnsValidationMessages_Succeeds()
        {
            var headers = new List<string>
                {
                    "Code",
                    "Name",
                    "Vix name",
                    "Easting",
                    "Northing",
                    "Latitude",
                    "Longitude",
                    "Marked on road?",
                    "In use Jan 2016?"
                };

            var rowOne = new List<string>
                {
                    "10001",
                    "Stop A",
                    "Stop A",
                    "1000",
                    "1000",
                    "1000.001",
                    "500.001",
                    "y",
                    "y"
                };

            var rowTwo = new List<string>
                {
                    "10001",
                    "Stop A",
                    "Stop A",
                    "1000",
                    "1000",
                    "1000.001",
                };

            var testData = new List<IList<string>> { headers, rowOne, rowTwo };
            var result = BusStopValidator.ValidatateCsv(testData);

            Assert.AreEqual(result.Count, 1);
            var r = result.First();
            Trace.Write($"Line: {r.Item1} - {r.Item2}");
        }


    }
}
