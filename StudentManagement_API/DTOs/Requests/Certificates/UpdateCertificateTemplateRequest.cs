namespace StudentManagement_API.DTOs.Requests
{
    public class UpdateCertificateTemplateRequest
    {
        public int InstituteID { get; set; }
        public int TemplateID { get; set; }
        public string TemplateName { get; set; }
        public string TemplateContent { get; set; }
    }
}
