using System.Collections.Generic;

namespace StudentManagement_API.DTOs.Responses
{
    public class GetStudentBirthdaysResponse
    {
        public List<StudentBirthday> Students { get; set; }
    }

    public class StudentBirthday
    {
        public string StudentName { get; set; }
        public string BirthDay { get; set; } // Formatted as "12 Dec 2024"
        public string Class { get; set; }
        public string Section { get; set; }
    }
}
