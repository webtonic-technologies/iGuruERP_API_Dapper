using System.Collections.Generic;

namespace StudentManagement_API.DTOs.Responses
{
    public class GetClassWiseStudentsResponse
    {
        public List<ClassWiseStudent> Students { get; set; }
    }

    public class ClassWiseStudent
    {
        public string ClassName { get; set; }
        public int BoysCount { get; set; }
        public int GirlsCount { get; set; }
    }
}
