namespace Lesson_API.DTOs.Responses
{
    public class GetAllHomeworkExportResponse
    {
        public int HomeworkID { get; set; }

        public string HomeworkName { get; set; }  // Name of the homework
        public string SubjectName { get; set; }   // Subject of the homework
        public string HomeworkType { get; set; }  // Type of homework (e.g., assignment, quiz)
        public string Notes { get; set; }         // Any notes associated with the homework
        public string CreatedBy { get; set; }     // Name of the creator of the homework
        public string CreatedOn { get; set; }     // Creation date of the homework in 'dd-MM-yyyy at hh:mm tt' format
        public List<ClassSectionResponse> ClassSections { get; set; } // List of class sections associated with the homework 
    }

    public class ClassSectionResponse
    {
        public string ClassName { get; set; }    // Name of the class (e.g., Class 10)
        public string SectionName { get; set; }  // Name of the section (e.g., Section A)
    }
}
