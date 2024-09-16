namespace Communication_API.DTOs.Requests.NoticeBoard
{
    public class AddUpdateCircularRequest
    {
        public int CircularID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Attachment { get; set; }
        public string CircularNo { get; set; }
        public DateTime CircularDate { get; set; }
        public DateTime PublishedDate { get; set; }
        public bool IsStudent { get; set; }
        public bool IsEmployee { get; set; }
        public bool ScheduleNow { get; set; }
        public DateTime ScheduleDate { get; set; }
        public DateTime ScheduleTime { get; set; }
        public int InstituteID { get; set; } // New field for InstituteID

        // Mappings
        public List<StudentMapping>? StudentMappings { get; set; }
        public List<EmployeeMapping>? EmployeeMappings { get; set; }
    }

}
