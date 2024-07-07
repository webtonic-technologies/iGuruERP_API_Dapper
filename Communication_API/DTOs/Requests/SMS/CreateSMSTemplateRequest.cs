namespace Communication_API.DTOs.Requests.SMS
{
    public class CreateSMSTemplateRequest
    {
        public string TemplateName { get; set; }
        public string TemplateMessage { get; set; }
    }
}
