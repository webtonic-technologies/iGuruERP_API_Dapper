namespace Communication_API.DTOs.Requests.PushNotification
{
    public class UpdatePushNotificationEmployeeStatusRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public int EmployeeID { get; set; }
        public int PushNotificationStatusID { get; set; }
    }
}
