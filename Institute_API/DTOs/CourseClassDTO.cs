using Institute_API.Models;

namespace Institute_API.DTOs
{
    public class CourseClassDTO
    {
        public int CourseClass_id { get; set; }
        public int Institute_id { get; set; }
        public string Class_course { get; set; } = string.Empty;
        public List<CourseClassSection>? CourseClassSections { get; set; }
    }
    public class GetAllCourseClassRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int Institute_id { get; set; }
    }
}
