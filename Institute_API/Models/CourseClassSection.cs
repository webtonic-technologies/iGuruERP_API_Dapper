namespace Institute_API.Models
{
    public class CourseClassSection
    {
        public int CourseClassSection_id {  get; set; }
        public int CourseClass_id {  get; set; }
        public string Section {  get; set; } = string.Empty;
    }
}
