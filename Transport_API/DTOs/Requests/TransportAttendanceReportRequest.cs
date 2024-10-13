namespace Transport_API.DTOs.Requests
{
    public class TransportAttendanceReportRequest
    {
        // Dates as strings in "DD-MM-YYYY" format
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int RoutePlanID { get; set; }
        public int InstituteID { get; set; }

        // Helper methods to convert the string date to DateTime for internal use
        public DateTime GetStartDateAsDateTime()
        {
            return DateTime.ParseExact(StartDate, "dd-MM-yyyy", null);
        }

        public DateTime GetEndDateAsDateTime()
        {
            return DateTime.ParseExact(EndDate, "dd-MM-yyyy", null);
        }
    }

}
