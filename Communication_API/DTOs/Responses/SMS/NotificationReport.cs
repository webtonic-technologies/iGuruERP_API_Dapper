namespace Communication_API.DTOs.Responses.SMS
{
    public class NotificationReport
    {
        // Fields for Student reports
        public int? StudentID { get; set; } // Nullable for Employee reports
        public string StudentName { get; set; } // Nullable for Employee reports
        public string ClassSection { get; set; } // Nullable for Employee reports

        // Fields for Employee reports
        public int? EmployeeID { get; set; } // Nullable for Student reports
        public string EmployeeName { get; set; } // Nullable for Student reports
        public string DepartmentDesignation { get; set; } // Nullable for Student reports

        // Common fields
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}
