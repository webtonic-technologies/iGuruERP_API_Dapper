namespace StudentManagement_API.DTOs.Responses
{
    public class GetCertificateTemplatesListResponse
    {
        public int TemplateID { get; set; }
        public string TemplateName { get; set; }
        public string TemplateContent { get; set; }
        public string CreatedOn { get; set; }  // e.g., "02 Mar, 2025"
    }
}
