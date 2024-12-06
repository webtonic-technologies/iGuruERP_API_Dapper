namespace Communication_API.DTOs.Responses.PushNotification
{
    public class PushNotificationStudentResponse
    {
        public int GroupID { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string PushNotificationMessage { get; set; }
        public DateTime PushNotificationDate { get; set; }
        public int PushNotificationStatusID { get; set; }
    }
}
