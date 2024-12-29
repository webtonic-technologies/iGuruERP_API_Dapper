namespace Lesson_API.DTOs.Responses
{
    public class ClassSectionHWResponse
    { 
        public int HomeworkID { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }

    public class ClassSectionASResponse
    {
        public int AssignmentID { get; set; } 
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }

    public class StudentASResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
    }
}
