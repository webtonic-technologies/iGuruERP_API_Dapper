namespace Lesson_API.DTOs.Requests
{
    public class GetLessonPlanningRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int SubjectID { get; set; }
        public int EmployeeID { get; set; }
        public int InstituteID { get; set; }
    }
}
