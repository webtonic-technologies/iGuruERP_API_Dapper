namespace Communication_API.DTOs.Requests.SMS
{
    public class GetSMSReportRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // Add these properties
        public int UserTypeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
