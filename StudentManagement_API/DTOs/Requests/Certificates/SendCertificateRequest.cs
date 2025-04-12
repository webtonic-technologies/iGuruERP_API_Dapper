namespace StudentManagement_API.DTOs.Requests
{
    public class CertificateWithStudent
    {
        public int StudentID { get; set; }
        public int CertificateID { get; set; }
    }

    public class SendCertificateRequest
    {
        public int InstituteID { get; set; }
        public List<CertificateWithStudent> Students { get; set; }
    }
}
