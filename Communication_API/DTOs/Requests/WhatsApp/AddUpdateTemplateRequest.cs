namespace Communication_API.DTOs.Requests.WhatsApp
{
    public class AddUpdateTemplateRequest
    {
        public int TemplateID { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateName { get; set; }
        public string TemplateMessage { get; set; }
    }

    public class AddUpdateWhatsAppTemplateRequest
    {
        public int WhatsAppTemplateID { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateName { get; set; }
        public string TemplateMessage { get; set; }
        public int InstituteID { get; set; }
    }
}
