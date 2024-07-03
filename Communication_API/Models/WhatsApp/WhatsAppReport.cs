namespace Communication_API.Models.WhatsApp
{
    public class WhatsAppReport
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
