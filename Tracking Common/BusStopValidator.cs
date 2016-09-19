using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracking_Common
{
    public class BusStopValidator
    {
        private static Tuple<int, string> Warn(int lineNumber, string message)
        {
            return new Tuple<int, string>(lineNumber, message);
        }

        private static bool IsInteger(string cell)
        {
            int intCheck;
            return int.TryParse(cell, out intCheck);
        }

        private static bool IsDouble(string cell)
        {
            double doubleCheck;
            return double.TryParse(cell, out doubleCheck);
        }

        private static bool IsDecimal(string cell)
        {
            decimal decimalCheck;
            return decimal.TryParse(cell, out decimalCheck);
        }

        /// <summary>
        /// Validates a csv upload to check it has been correctly formatted
        /// </summary>
        /// <param name="csvContent"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IList<Tuple<int, string>> ValidatateCsv(IList<IList<string>> csvContent)
        {
            var validationMessages = new List<Tuple<int, string>>();
            if (csvContent.Count <= 1) { validationMessages.Add(Warn(1, $"Row count must be greater than 1 but was equal to {csvContent.Count}.")); }
            var columnCount = csvContent.First().Count;
            if (columnCount != 9) { validationMessages.Add(Warn(1, $"Column count must 9 but was equal to {columnCount}.")); }
            if (validationMessages.Any()) return validationMessages;

            var lineCount = 2;
            foreach (var cells in csvContent.Skip(1).Select(line => line))
            {
                if (cells.Count != 9)
                {
                    validationMessages.Add(Warn(lineCount, $"Incorrect column count, should be 9 but was {cells.Count}."));
                    continue;
                }
                if (!IsInteger(cells[0]))
                {
                    validationMessages.Add(Warn(lineCount, $"Bus code was not a valid integer, value was '{cells[0]}'."));
                }
                if (cells[1] == null || cells[1] == "")
                {
                    validationMessages.Add(Warn(lineCount, "Bus name was null or whitespace."));
                }
                if (cells[2] == null || cells[2] == "")
                {
                    validationMessages.Add(Warn(lineCount, "Vix name was null or whitespace."));
                }
                if (!IsInteger(cells[3]))
                {
                    validationMessages.Add(Warn(lineCount, $"Easting was not a valid integer, value was '{cells[3]}'."));
                }
                if (!IsInteger(cells[4]))
                {
                    validationMessages.Add(Warn(lineCount, $"Northing was not a valid integer, value was '{cells[4]}'."));
                }
                if (!IsDecimal(cells[5]))
                {
                    validationMessages.Add(Warn(lineCount, $"Latitude was not a valid decimal, value was '{cells[5]}'."));
                }
                if (!IsDecimal(cells[6]))
                {
                    validationMessages.Add(Warn(lineCount, $"Longitude was not a valid decimal, value was '{cells[6]}'."));
                }
                if (cells[7] != "y" && cells[7] != "n")
                {
                    validationMessages.Add(Warn(lineCount, $"Marked on road must be 'y' or 'n', value was '{cells[7]}'."));
                }
                if (cells[8] != "y" && cells[8] != "n")
                {
                    validationMessages.Add(Warn(lineCount, $"Is in use must be 'y' or 'n', value was '{cells[8]}'."));
                }
                lineCount++;
            }
            return validationMessages;
        }
    }
}

