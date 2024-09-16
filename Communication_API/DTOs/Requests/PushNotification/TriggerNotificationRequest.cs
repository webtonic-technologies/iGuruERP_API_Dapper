namespace Communication_API.DTOs.Requests.PushNotification
{
    public class TriggerNotificationRequest
    {
        public int PushNotificationID { get; set; }
        public int PredefinedTemplateID { get; set; }
        public string NotificationMessage { get; set; }
        public int UserTypeID { get; set; }
        public int GroupID { get; set; }
        public bool Status { get; set; }
        public bool ScheduleNow { get; set; }
        public DateTime ScheduleDate { get; set; }
        public DateTime ScheduleTime { get; set; }

        // Lists for student and employee IDs
        public List<int>? StudentIDs { get; set; }
        public List<int>? EmployeeIDs { get; set; }
    }


}
