using System.Collections.Generic;

namespace Lesson_API.DTOs.Responses
{
    public class GetAllCurriculumResponse
    {
        public string Subject { get; set; }
        public string ClassSectionNames { get; set; }
        public int NumberOfChapters { get; set; }
    }
}
