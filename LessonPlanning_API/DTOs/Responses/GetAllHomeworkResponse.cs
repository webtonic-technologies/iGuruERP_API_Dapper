using Lesson_API.DTOs.Requests;

namespace Lesson_API.DTOs.Responses
{
    public class GetAllHomeworkResponse
    {
        public int HomeworkID { get; set; }
        public string HomeworkName { get; set; }
        public string SubjectName { get; set; }   // Get from tbl_Subjects
        public string HomeworkType { get; set; }  // Get from tblHomeworkType
        public string Notes { get; set; }
        public string CreatedBy { get; set; }     // Get from tbl_EmployeeProfileMaster
        public string CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public List<ClassSectionHWResponse> ClassSections { get; set; }
        public List<HomeworkDocs> HomeworkDocs { get; set; } // List of documents associated with homework

    }
}
