namespace Communication_API.DTOs.Requests.PushNotification
{
    public class SendPushNotificationEmployeeRequest
    {

        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public List<NotificationMessageEMP> EmployeeNotification { get; set; }  // Renamed to avoid conflict
        public string NotificationDate { get; set; } // Changed to string to match request format
        public int SentBy { get; set; }

        //public int GroupID { get; set; }
        //public int InstituteID { get; set; }
        //public List<int> EmployeeIDs { get; set; }  // List of Employee IDs
        //public string PushNotificationMessage { get; set; }
        //public DateTime PushNotificationDate { get; set; }
    }

    public class NotificationMessageEMP
    {
        public int EmployeeID { get; set; }
        public string Message { get; set; }  // Renamed property to avoid conflict
    }
}
