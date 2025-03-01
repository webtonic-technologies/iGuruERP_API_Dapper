namespace StudentManagement_API.DTOs.Requests
{
    public class CreateCertificateTemplateRequest
    {
        public string TemplateName { get; set; }
        public string TemplateContent { get; set; }
        public int InstituteID { get; set; }
        public int UserID { get; set; }
    }
}
