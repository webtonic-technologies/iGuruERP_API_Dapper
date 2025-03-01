namespace StudentManagement_API.DTOs.Responses
{
    public class GetCertificateTemplateResponse
    {
        public int TemplateID { get; set; }
        public string TemplateName { get; set; }
        public string UserName { get; set; }
        public string CreatedOn { get; set; }
    }
}
