namespace Student_API.DTOs
{
 
    public class TimeTableGroupDTO
    {
        public int TimetableGroup_id { get; set; }
        public string GroupName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<PeriodDTO>periodDTOs { get; set; }  
        public List<PeriodBreakDTO> periodBreakDTOs { get; set; }  
        public List<TimetableClassMapping> timetableClassMappings { get; set; } 
    }

  
    public class PeriodDTO
    {
        public int Period_id { get; set; }
        public int TimetableGroup_id { get; set; }
        public string PeriodName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

 
    public class PeriodBreakDTO
    {
        public int PeriodBreak_id { get; set; }
        public int TimetableGroup_id { get; set; }
        public string BreakName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class TimetableClassMapping
    {
        public int TimetableClassMapping_id { get; set; }
        public int TimetableGroup_id { get; set; }
        public int Class_id { get; set; }
        public int Section_id { get; set; }
    }
}
