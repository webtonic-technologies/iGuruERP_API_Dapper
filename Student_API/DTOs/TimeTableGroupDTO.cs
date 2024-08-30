using Student_API.Helper;

namespace Student_API.DTOs
{
 
    public class TimeTableGroupDTO
    {
        public int TimetableGroup_id { get; set; }
        public string GroupName { get; set; }
        [ValidDateString("HH:mm")]
        public string StartTime { get; set; }
        [ValidDateString("HH:mm")]
        public string EndTime { get; set; }
        public List<PeriodDTO>periodDTOs { get; set; }  
        public List<PeriodBreakDTO> periodBreakDTOs { get; set; }  
        public List<TimetableClassMapping> timetableClassMappings { get; set; } 
        public int InstituteId { get; set; }    
    }

    public class TimeTableGroupDTOResponse
    {
        public int TimetableGroup_id { get; set; }
        public string GroupName { get; set; }
        [ValidDateString("HH:mm")]
        public string StartTime { get; set; }
        [ValidDateString("HH:mm")]
        public string EndTime { get; set; }
        public List<PeriodDTO> periodDTOs { get; set; }
        public List<PeriodBreakDTO> periodBreakDTOs { get; set; }
        public List<TimetableClassMappingResponse> timetableClassMappings { get; set; }
        public int InstituteId { get; set; }
    }


    public class PeriodDTO
    {
        public int Period_id { get; set; }
        public int TimetableGroup_id { get; set; }
        public string PeriodName { get; set; }
        [ValidDateString("HH:mm")]
        public string StartTime { get; set; }
        [ValidDateString("HH:mm")]
        public string EndTime { get; set; }
    }

 
    public class PeriodBreakDTO
    {
        public int PeriodBreak_id { get; set; }
        public int TimetableGroup_id { get; set; }
        public string BreakName { get; set; }
        [ValidDateString("HH:mm")]
        public string StartTime { get; set; }
        [ValidDateString("HH:mm")]
        public string EndTime { get; set; }
    }

    public class TimetableClassMapping
    {
        public int TimetableClassMapping_id { get; set; }
        public int TimetableGroup_id { get; set; }
        public int Class_id { get; set; }
        public int Section_id { get; set; }
       
    }

    public class TimetableClassMappingResponse
    {
        public int TimetableClassMapping_id { get; set; }
        public int TimetableGroup_id { get; set; }
        public int Class_id { get; set; }
        public int Section_id { get; set; }
        public string class_name { get; set; }
        public string Section_name { get; set; }
    }
}
