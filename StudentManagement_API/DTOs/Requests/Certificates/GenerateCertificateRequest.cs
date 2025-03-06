//namespace StudentManagement_API.DTOs.Requests
//{
//    public class GenerateCertificateRequest
//    {
//        public int TemplateID { get; set; }
//        public int StudentID { get; set; }
//        public int InstituteID { get; set; }
//        public string CerficateContent { get; set; }
//        public string CertificateFile { get; set; } // Base64 image string
//    }
//}


//using System.Collections.Generic;

//namespace StudentManagement_API.DTOs.Requests
//{
//    public class GenerateCertificateRequest
//    {
//        public int TemplateID { get; set; }
//        public int InstituteID { get; set; }
//        public List<CertificateDetail> Cerficates { get; set; }
//    }

//    public class CertificateDetail
//    {
//        public int StudentID { get; set; }
//        public string CerficateContent { get; set; }
//        public string CertificateFile { get; set; } // Base64 image string
//    }
//}

using System.Collections.Generic;

namespace StudentManagement_API.DTOs.Requests
{
    public class GenerateCertificateRequest
    {
        public int TemplateID { get; set; }
        public int InstituteID { get; set; }
        public List<int> StudentIDs { get; set; }
    }
}
