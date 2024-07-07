namespace Communication_API.DTOs.Requests.WhatsApp
{
    public class SendWhatsAppRequest
    {
        public int PredefinedTemplateID { get; set; }
        public string WhatsAppMessage { get; set; }
        public int UserTypeID { get; set; }
        public int GroupID { get; set; }
        public bool ScheduleNow { get; set; }
        public DateTime ScheduleDate { get; set; }
        public TimeSpan ScheduleTime { get; set; }
    }
}
