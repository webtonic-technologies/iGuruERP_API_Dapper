﻿namespace Communication_API.DTOs.Responses.Email
{
    public class EmailEmployeeReportExportResponse
    {
        public string EmployeeName { get; set; }
        public string DepartmentDesignation { get; set; }
        public string DateTime { get; set; }  // Format '15 Dec 2024, 05:00 PM'
        public string EmailSubject { get; set; }
        public string EmailID { get; set; }
        public string Status { get; set; }
        public string SentBy { get; set; }

    }
}
