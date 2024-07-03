namespace Communication_API.Models.PushNotification
{
    public class Notification
    {
        public int PushNotificationID { get; set; }
        public int PredefinedTemplateID { get; set; }
        public string? NotificationMessage { get; set; }
        public int UserTypeID { get; set; }
        public int GroupID { get; set; }
        public bool Status { get; set; }
        public bool ScheduleNow { get; set; }
        public DateTime ScheduleDate { get; set; }
        public DateTime ScheduleTime { get; set; }
    }
}
