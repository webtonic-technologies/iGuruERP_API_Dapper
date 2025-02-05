namespace Communication_API.DTOs.Responses.PushNotification
{
    public class GetNotificationEmployeeReportResponse
    {
        public int? EmployeeID { get; set; } // Nullable for Employee reports 
        public string EmployeeName { get; set; } // Nullable for Employee reports
        public string DepartmentDesignation { get; set; } // Nullable for Employee reports   
        public string DateTime { get; set; }  // Format '15 Dec 2024, 05:00 PM'
        public string Message { get; set; }
        public string Status { get; set; }
        public string SentBy { get; set; }
    }
}
