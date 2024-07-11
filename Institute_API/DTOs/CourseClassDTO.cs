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
        public string SearchText { get; set; } = string.Empty;
    }
    public class Class
    {
        public int class_id { get; set; }
        public string class_name { get; set; } = string.Empty;
        public int institute_id { get; set; }
        public bool IsDeleted {  get; set; }
        public List<Section>? Sections {  get; set; }
    }
    public class Section
    {
        public int section_id { get; set; }
        public string section_name { get; set; } = string.Empty;
        public int class_id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
