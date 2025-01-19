namespace Communication_API.DTOs.Requests.WhatsApp
{
    public class GetWhatsAppTemplateRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int InstituteID { get; set; }
    }
}
