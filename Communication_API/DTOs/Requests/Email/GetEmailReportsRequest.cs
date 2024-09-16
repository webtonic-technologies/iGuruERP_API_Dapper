namespace Communication_API.DTOs.Requests.Email
{
    public class GetEmailReportsRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // Add these properties to fix the errors
        public int UserTypeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
