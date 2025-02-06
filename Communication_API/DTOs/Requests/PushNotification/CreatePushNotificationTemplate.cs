namespace Communication_API.DTOs.Requests.PushNotification
{
    public class CreatePushNotificationTemplate
    {
        public string TemplateName { get; set; }
        public string TemplateMessage { get; set; }
        public int InstituteID { get; set; } 
    }
}
