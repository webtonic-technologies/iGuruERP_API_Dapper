namespace Communication_API.DTOs.Requests.NoticeBoard
{
    public class AddUpdateCircularRequest
    {
        public int CircularID { get; set; }
        public string AcademicYear { get; set; } // New field for Academic Year
        public string Title { get; set; }
        public string Message { get; set; }
        public string Attachment { get; set; }
        public string CircularNo { get; set; }
        public string CircularDate { get; set; }
        public string PublishedDate { get; set; }
        public bool IsStudent { get; set; }
        public bool IsEmployee { get; set; }
        public bool ScheduleNow { get; set; }
        public string ScheduleDate { get; set; }
        public string ScheduleTime { get; set; }
        public int InstituteID { get; set; } // New field for InstituteID
        public int CreatedBy { get; set; }

        // Mappings
        public List<StudentMapping>? StudentMappings { get; set; }
        public List<EmployeeMapping>? EmployeeMappings { get; set; }
    }

}
