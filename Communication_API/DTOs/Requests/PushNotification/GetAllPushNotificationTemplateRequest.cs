namespace Communication_API.DTOs.Requests.PushNotification
{
    public class GetAllPushNotificationTemplateRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
