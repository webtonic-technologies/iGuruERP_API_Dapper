namespace Communication_API.DTOs.Requests.WhatsApp
{
    public class GetWhatsAppReportRequest
    {
        public int UserTypeID { get; set; } // 1 for Student, 2 for Employee
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

    }
}
