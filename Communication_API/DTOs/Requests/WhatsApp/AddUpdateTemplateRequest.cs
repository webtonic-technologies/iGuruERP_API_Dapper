namespace Communication_API.DTOs.Requests.WhatsApp
{
    public class AddUpdateTemplateRequest
    {
        public int TemplateID { get; set; }
        public int TemplateTypeID { get; set; }
        public string TemplateName { get; set; }
        public string TemplateMessage { get; set; }
    }
}
