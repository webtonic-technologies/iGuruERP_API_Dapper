﻿using Institute_API.Helper;

namespace Institute_API.DTOs
{
    public class EventDTO
    {
        public int Event_id { get; set; }
        public string EventName { get; set; }
        [ValidDateString("dd-MM-yyyy hh:mm tt")]
        public string StartDate { get; set; }
        [ValidDateString("dd-MM-yyyy hh:mm tt")]
        public string EndDate { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string ScheduleTime { get; set; }
        public string ScheduleDate { get; set; }
        public string? AttachmentFile { get; set; }
        public int Institute_id { get; set; }
        public int Academic_year_id { get; set; }
        public string YearName { get; set; }
        public List<EventFileMapping> AttachmentFiles {  get; set; }    
        public List<EventEmployeeMapping> EmployeeMappings { get; set; }
        public List<EventClassSessionMapping> ClassSessionMappings { get; set; }
    }
    public class EventEmployeeMapping
    {
        public int EventEmployeeMapping_id { get; set; }
        public int Event_id { get; set; }
        public int Employee_id { get; set; }
        public string? Employee_Name { get; set; }
    }

    public class EventClassSessionMapping
    {
        public int EventClassSessionMapping_id { get; set; }
        public int Event_id { get; set; }
        public int Class_id { get; set; }
        public int Section_id { get; set; }
        public string? section_name { get; set; }
        public string? class_name { get; set; }
    }
    public class EventFileMapping
    {
        public int EventFileMapping_Id { get; set; }
        public int Event_id { get; set; }
        public string attachment { get; set; }
    }
}
