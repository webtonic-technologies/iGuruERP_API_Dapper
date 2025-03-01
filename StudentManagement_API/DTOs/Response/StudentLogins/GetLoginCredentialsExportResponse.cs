using System;

namespace StudentManagement_API.DTOs.Responses
{
    public class GetLoginCredentialsExportResponse
    { 
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public string LoginID { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public DateTime? LastActionTaken { get; set; }
        public string AppVersion { get; set; }
    }
}
