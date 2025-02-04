namespace Communication_API.DTOs.Responses.WhatsApp
{
    public class WhatsAppEmployeeReportsResponse
    {   // Fields for Employee reports
        public int? EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentDesignation { get; set; }
        public string DateTime { get; set; }  // Format '15 Dec 2024, 05:00 PM'
        public string Message { get; set; }
        public string Status { get; set; }
        public string SentBy { get; set; }

    }
}
