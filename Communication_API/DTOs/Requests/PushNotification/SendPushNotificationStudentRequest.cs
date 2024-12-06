namespace Communication_API.DTOs.Requests.PushNotification
{
    public class SendPushNotificationStudentRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public List<int> StudentIDs { get; set; }
        public string PushNotificationMessage { get; set; }
        public DateTime PushNotificationDate { get; set; }
    }
}
