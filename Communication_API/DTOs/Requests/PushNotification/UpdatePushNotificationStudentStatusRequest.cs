namespace Communication_API.DTOs.Requests.PushNotification
{
    public class UpdatePushNotificationStudentStatusRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public int StudentID { get; set; }
        public int PushNotificationStatusID { get; set; }
    }
}
