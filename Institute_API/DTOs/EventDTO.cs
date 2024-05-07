namespace Institute_API.DTOs
{
    public class EventDTO
    {
        public int Event_id { get; set; }
        public string EventName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string ScheduleTime { get; set; }
        public string Time { get; set; }
        public string AttachmentFile { get; set; }
        public bool isApproved { get; set; }
        public string approvedBy { get; set; }

        public List<EventEmployeeMapping> EmployeeMappings { get; set; }
        public List<EventClassSessionMapping> ClassSessionMappings { get; set; }
    }
    public class EventEmployeeMapping
    {
        public int EventEmployeeMapping_id { get; set; }
        public int Event_id { get; set; }
        public int Employee_id { get; set; }
    }

    public class EventClassSessionMapping
    {
        public int EventClassSessionMapping_id { get; set; }
        public int Event_id { get; set; }
        public int Class_id { get; set; }
        public int Section_id { get; set; }
    }
}
