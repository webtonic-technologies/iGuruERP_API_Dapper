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
        public DateTime ScheduleDate { get; set; }
        public DateTime ScheduleTime { get; set; }

        // Add these properties
        public List<int>? StudentIDs { get; set; } // For userTypeID = 1
        public List<int>? EmployeeIDs { get; set; } // For userTypeID = 2
    }

}
