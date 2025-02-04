namespace Communication_API.DTOs.Requests.WhatsApp
{
    public class SendWhatsAppRequest
    {
        public int WhatsAppMessageID { get; set; }
        public int PredefinedTemplateID { get; set; }
        public string WhatsAppMessage { get; set; }
        public int UserTypeID { get; set; }
        public int GroupID { get; set; }
        public bool Status { get; set; }
        public bool ScheduleNow { get; set; }

        // ScheduleDate and ScheduleTime changed to string
        // Format: DD-MM-YYYY for ScheduleDate and HH:mm tt for ScheduleTime
        public string ScheduleDate { get; set; }
        public string ScheduleTime { get; set; }

        // Add these properties
        public List<int>? StudentIDs { get; set; } 
        public List<int>? EmployeeIDs { get; set; } 

        // New properties
        public string AcademicYearCode { get; set; } 
        public int InstituteID { get; set; }
        public int SentBy { get; set; }

    }
}
