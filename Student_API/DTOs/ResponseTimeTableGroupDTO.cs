namespace Student_API.DTOs
{
    public class ResponseTimeTableGroupDTO
    {
        public int TimetableGroupId { get; set; }
        public string GroupName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int NumberOfPeriods { get; set; }
        public int NumberOfBreaks { get; set; }
        public List<TimetableClassMappingDTO> timetableClassMappingDTOs { get; set; }
    }

    public class TimetableClassMappingDTO
    {
        public int TimetableClassMapping_id { get; set; }
        public int Class_id { get; set; }
        public int Section_id { get; set; }
        public string class_name { get; set; }
        public string Section_name { get; set; }
    }
}
