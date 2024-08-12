namespace Student_API.DTOs
{
    public class ClassWiseGenderCountDTO
    {
        public string ClassName { get; set; }
        public int BoysCount { get; set; }
        public int GirlsCount { get; set; }
    }

    public class CourseClassDTO
    {
        public int CourseClassId { get; set; }
        public string ClassName { get; set; }
    }

    public class CourseClassSectionDTO
    {
        public int CourseClassSectionId { get; set; }
        public string SectionName { get; set; }
    }
}
    