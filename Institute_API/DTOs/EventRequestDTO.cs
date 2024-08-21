using Institute_API.Helper;

namespace Institute_API.DTOs
{
    public class EventRequestDTO
    {
        public int Event_id { get; set; }
        public string EventName { get; set; }
        [ValidDateString("dd-MM-yyyy hh:mm tt")]
        public string StartDate { get; set; }
        [ValidDateString("dd-MM-yyyy hh:mm tt")]
        public string EndDate { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        [ValidDateString("dd-MM-yyyy")]
        public string ScheduleDate { get; set; }

        [ValidDateString("hh:mm tt")]
        public string ScheduleTime { get; set; }
        public string AttachmentFile { get; set; }
        public int Institute_id { get; set; }
        public int Academic_year_id { get; set; }
        public int CreatedBy { get; set; }

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
