namespace Lesson_API.DTOs.Requests
{
    public class GetAllLessonPlanningRequest : BaseRequest
    {
        public string AcademicYearID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int SubjectID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int InstituteID { get; set; }
    }
}
