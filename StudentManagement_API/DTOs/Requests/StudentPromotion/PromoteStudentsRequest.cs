using System.Collections.Generic;

namespace StudentManagement_API.DTOs.Requests
{
    public class PromoteStudentsRequest
    {
        public int InstituteID { get; set; }
        public string AcademicYearCode { get; set; }
        public List<int> Students { get; set; }
        public ClassInfo CurrentClass { get; set; }
        public ClassInfo NextClass { get; set; }
    }

    public class ClassInfo
    {
        public int ClassID { get; set; }
        public List<int> Sections { get; set; }
    }
}
