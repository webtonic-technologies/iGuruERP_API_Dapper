namespace Communication_API.DTOs.Responses.WhatsApp
{
    public class WhatsAppReportResponse
    {
        public int WhatsAppMessageID { get; set; }
        public int PredefinedTemplateID { get; set; }
        public string WhatsAppMessage { get; set; }
        public int UserTypeID { get; set; }
        public int GroupID { get; set; }
        public bool Status { get; set; }
        public DateTime ScheduleDate { get; set; }
        public TimeSpan ScheduleTime { get; set; }
    }
}
