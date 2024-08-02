using System.Collections.Generic;

namespace Lesson_API.Models
{
    public class LessonPlanning
    {
        public int LessonPlanningID { get; set; }
        public string AcademicYear { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int SubjectID { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<LessonPlanningInformation> LessonPlanningInformation { get; set; }
    }
}
