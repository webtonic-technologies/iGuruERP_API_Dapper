using System.Globalization;

namespace Attendance_API.Helper
{
    public class DateTimeHelper
    {
        public static DateTime ConvertToDateTime(string dateString, string format = "dd-MM-yyyy hh:mm tt")
        {
            if (string.IsNullOrEmpty(dateString))
            {
                throw new ArgumentException("Date string cannot be null or empty", nameof(dateString));
            }

            DateTime parsedDate;
            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return parsedDate;
            }
            else
            {
                throw new FormatException("Invalid date format");
            }
        }
    }
}
