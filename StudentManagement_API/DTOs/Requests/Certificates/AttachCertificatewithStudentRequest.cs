namespace StudentManagement_API.DTOs.Requests
{
    public class AttachCertificatewithStudentRequest
    {
        public int InstituteID { get; set; }
        public int TemplateID { get; set; }
        public int StudentID { get; set; }
        public string Certificate { get; set; } // Base64 image string
    }
}
