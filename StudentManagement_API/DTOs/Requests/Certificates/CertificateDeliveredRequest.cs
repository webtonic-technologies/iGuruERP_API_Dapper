namespace StudentManagement_API.DTOs.Requests
{
    public class CertificateDeliveredRequest
    {
        public int StudentID { get; set; }
        public int CertificateID { get; set; }
        public int InstituteID { get; set; }
    }
}
