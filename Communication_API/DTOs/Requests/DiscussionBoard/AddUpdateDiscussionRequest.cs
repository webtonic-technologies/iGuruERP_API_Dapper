using System.Collections.Generic;

namespace Communication_API.DTOs.Requests.DiscussionBoard
{
    public class AddUpdateDiscussionRequest
    {
        public int DiscussionBoardID { get; set; } // 0 for insert; non-zero for update
        public string DiscussionHeading { get; set; }
        public string Description { get; set; }
        public string Attachments { get; set; }
        public string StartDate { get; set; }  // Expected format: "dd-MM-yyyy"
        public string EndDate { get; set; }    // Expected format: "dd-MM-yyyy"
        public bool IsStudent { get; set; }
        public bool IsEmployee { get; set; }
        public List<DBStudentMapping> StudentMappings { get; set; }
        public List<DBEmployeeMapping> EmployeeMappings { get; set; }
        public int CreatedBy { get; set; }
        public int InstituteID { get; set; }
    }

    public class DBStudentMapping
    {
        public int StudentID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }

    public class DBEmployeeMapping
    {
        public int EmployeeID { get; set; }
        public int DepartmentID { get; set; }
        public int DesignationID { get; set; }
    }
}
