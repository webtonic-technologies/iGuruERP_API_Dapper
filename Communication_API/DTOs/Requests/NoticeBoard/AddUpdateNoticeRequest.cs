namespace Communication_API.DTOs.Requests.NoticeBoard
{
    public class AddUpdateNoticeRequest
    {
        public int InstituteID { get; set; } // Add this property
        public int NoticeID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Attachments { get; set; }
        public string StartDate { get; set; }  // Change to string
        public string EndDate { get; set; }    // Change to string
        public bool IsStudent { get; set; }
        public bool IsEmployee { get; set; }
        public bool ScheduleNow { get; set; }
        public string ScheduleDate { get; set; }  // Change to string
        public string ScheduleTime { get; set; }  // Change to string

        // Add these properties to support mappings
        public List<StudentMapping>? StudentMappings { get; set; }
        public List<EmployeeMapping>? EmployeeMappings { get; set; }
    }

    public class StudentMapping
    {
        public int StudentID { get; set; } // Add this property
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }

    public class EmployeeMapping
    {
        public int EmployeeID { get; set; } // Add this property
        public int DepartmentID { get; set; }
        public int DesignationID { get; set; }
    }
}
