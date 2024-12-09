using System.Collections.Generic;

namespace Lesson_API.DTOs.Responses
{
    public class GetAllCurriculumResponse
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public List<ClassSection> ClassSection { get; set; }
        public int NoOfChapters { get; set; }
    }

    public class ClassSection
    {
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }

}
