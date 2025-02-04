namespace Communication_API.DTOs.Requests.SMS
{
    public class SendNewSMSRequest
    {
        public int SMSID { get; set; }
        public int PredefinedTemplateID { get; set; }
        public string SMSMessage { get; set; }
        public int UserTypeID { get; set; }
        public int GroupID { get; set; }
        public bool Status { get; set; }
        public bool ScheduleNow { get; set; }

        // Changed to string with format DD-MM-YYYY
        public string ScheduleDate { get; set; }

        // Changed to string with format HH:mm tt (e.g., 02:00 PM)
        public string ScheduleTime { get; set; }

        // Add these properties
        public List<int>? StudentIDs { get; set; }
        public List<int>? EmployeeIDs { get; set; }

        // Add new properties for AcademicYearCode and InstituteID
        public string? AcademicYearCode { get; set; }
        public int? InstituteID { get; set; }
        public int SentBy { get; set; }
    }
}
