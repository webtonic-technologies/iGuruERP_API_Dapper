namespace Communication_API.Models.PushNotification
{
    public class Notification
    {
        // For students
        public int? StudentID { get; set; }
        public string StudentName { get; set; }
        public string ClassSection { get; set; }

        // For employees
        public int? EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentDesignation { get; set; }

        // Common fields
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
    }

}
