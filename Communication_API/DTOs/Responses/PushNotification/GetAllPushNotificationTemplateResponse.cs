namespace Communication_API.DTOs.Responses.PushNotification
{
    public class GetAllPushNotificationTemplateResponse
    {
        
        public int PushNotificationTemplateID { get; set; } 
        public string TemplateName { get; set; }
        public string TemplateMessage { get; set; }
    }
}
