using Lesson_API.DTOs.Requests;

namespace Lesson_API.DTOs.Responses
{
    public class GetAllHomeworkResponse
    {
        public int HomeworkID { get; set; }
        public string HomeworkName { get; set; }
        public string SubjectName { get; set; }
        public string HomeworkType { get; set; }
        public string Notes { get; set; }
        public string Attachments { get; set; }
        public bool IsActive { get; set; }
        public List<ClassSectionResponse> ClassSections { get; set; }
        public List<HomeworkDocs> HomeworkDocs { get; set; }
    }
}
