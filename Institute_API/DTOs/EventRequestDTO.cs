namespace Institute_API.DTOs
{
    public class EventRequestDTO
    {
        public int Event_id { get; set; }
        public string EventName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string ScheduleDate { get; set; }
        public string ScheduleTime { get; set; }
        public string? AttachmentFile { get; set; }
        public int Institute_id { get; set; }

        public List<EventEmployeeRequestMapping> EmployeeMappings { get; set; }
        public List<EventClassSessionRequestMapping> ClassSessionMappings { get; set; }
    }
    public class EventEmployeeRequestMapping
    {
        public int EventEmployeeMapping_id { get; set; }
        public int Event_id { get; set; }
        public int Employee_id { get; set; }
    }

    public class EventClassSessionRequestMapping
    {
        public int EventClassSessionMapping_id { get; set; }
        public int Event_id { get; set; }
        public int Class_id { get; set; }
        public int Section_id { get; set; }
    }
}
