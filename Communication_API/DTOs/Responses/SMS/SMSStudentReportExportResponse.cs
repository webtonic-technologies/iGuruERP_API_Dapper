namespace Communication_API.DTOs.Responses.SMS
{
    public class SMSStudentReportExportResponse
    {
        public string AdmissionNumber { get; set; }
        public string StudentName { get; set; }
        public string ClassSection { get; set; }
        public string DateTime { get; set; }  // Format '15 Dec 2024, 05:00 PM'
        public string Message { get; set; }
        public string Status { get; set; }
    }
}
