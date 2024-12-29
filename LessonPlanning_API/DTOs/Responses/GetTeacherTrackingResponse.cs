namespace Lesson_API.DTOs.Responses
{
    public class GetTeacherTrackingResponse
    {
        public int EmployeeID { get; set; }
        public string Teacher { get; set; }
        public List<TTClassSection> ClassSections { get; set; }
        public List<string> Subjects { get; set; }
    }

    public class TTClassSection
    {
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }

    public class TeacherTrackingQueryResult
    {
        public int EmployeeID { get; set; }
        public string Teacher { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string Subjects { get; set; }
    }
}
