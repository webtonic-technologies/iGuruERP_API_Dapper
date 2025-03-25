namespace StudentManagement_API.DTOs.Responses
{
    public class GetCertificateTemplateByIDResponse
    {
        public int TemplateID { get; set; }
        public string TemplateName { get; set; }
        public string UserName { get; set; }
        // CreatedOn will be formatted as "dd-MM-yyyy at hh:mm tt"
        public string CreatedOn { get; set; }
        public int InstituteID { get; set; }
    }
}
