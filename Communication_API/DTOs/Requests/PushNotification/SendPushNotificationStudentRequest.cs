namespace Communication_API.DTOs.Requests.PushNotification
{
    public class SendPushNotificationStudentRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public List<NotificationMessage> StudentNotification { get; set; }  // Renamed to avoid conflict
        public string NotificationDate { get; set; } // Changed to string to match request format
        public int SentBy { get; set; }

        //public int GroupID { get; set; }
        //public int InstituteID { get; set; }
        //public List<int> StudentIDs { get; set; }
        //public string PushNotificationMessage { get; set; }
        //public DateTime PushNotificationDate { get; set; }
    }


    public class NotificationMessage
    {
        public int StudentID { get; set; }
        public string Message { get; set; }  // Renamed property to avoid conflict
    }
}
