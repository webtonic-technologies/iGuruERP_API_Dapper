namespace Communication_API.DTOs.Requests.SMS
{
    public class CreateSMSTemplateRequest
    {
        public string TemplateName { get; set; }
        public string TemplateMessage { get; set; }
        public int InstituteID { get; set; }
        public string TemplateCode { get; set; }
    }
}
