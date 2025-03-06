//using System.Collections.Generic;

//namespace StudentManagement_API.DTOs.Responses
//{
//    public class GenerateCertificateResponse
//    {
//        public List<string> StudentCertificates { get; set; }
//    }
//}


using System.Collections.Generic;

namespace StudentManagement_API.DTOs.Responses
{
    public class StudentCertificateResponse
    {
        public int TemplateID { get; set; }
        public int StudentID { get; set; }
        public string Certificate { get; set; }
    }

    public class GenerateCertificateResponse
    {
        public List<StudentCertificateResponse> StudentCertificate { get; set; }
    }
}
