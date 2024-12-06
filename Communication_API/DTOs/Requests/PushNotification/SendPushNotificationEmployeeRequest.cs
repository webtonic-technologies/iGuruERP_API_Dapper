namespace Communication_API.DTOs.Requests.PushNotification
{
    public class SendPushNotificationEmployeeRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public List<int> EmployeeIDs { get; set; }  // List of Employee IDs
        public string PushNotificationMessage { get; set; }
        public DateTime PushNotificationDate { get; set; }
    }
}
