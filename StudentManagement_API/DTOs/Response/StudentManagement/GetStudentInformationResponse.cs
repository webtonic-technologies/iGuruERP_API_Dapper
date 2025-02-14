using System.Xml.Linq;

namespace StudentManagement_API.DTOs.Responses
{
    public class GetStudentInformationResponse
    {
        public int StudentID { get; set; }
        public string AdmissionNumber { get; set; }
        public string StudentName { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public string RollNumber { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string Religion { get; set; }
        public string DateOfJoining { get; set; }
        public string FatherName { get; set; }
        public string SiblingFirstName { get; set; }
        public string SiblingLastName { get; set; }
        public string SiblingAdmissionNumber { get; set; }
        public string SiblingClass { get; set; }
        public string SiblingSection { get; set; }
        public string SiblingInstituteName { get; set; }
        public string SiblingAadharNumber { get; set; }
    }
}
