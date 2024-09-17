namespace Communication_API.DTOs.Responses.PushNotification
{
    public class StudentNotificationReportResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string ClassSection { get; set; }
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
    }

    public class EmployeeNotificationReportResponse
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentDesignation { get; set; }
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
    }

}
