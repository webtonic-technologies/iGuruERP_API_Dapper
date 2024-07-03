namespace Communication_API.DTOs.Responses.WhatsApp
{
    public class WhatsAppTemplateResponse
    {
        public int TemplateID { get; set; }
        public int TemplateTypeID { get; set; }
        public string TemplateName { get; set; }
        public string TemplateMessage { get; set; }
    }
}
