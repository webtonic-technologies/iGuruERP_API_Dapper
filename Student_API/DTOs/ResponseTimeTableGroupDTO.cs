namespace Student_API.DTOs
{
    public class ResponseTimeTableGroupDTO
    {
        public int TimetableGroupId { get; set; }
        public string GroupName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan  EndTime { get; set; }
        public int NumberOfPeriods { get; set; }
        public int NumberOfBreaks { get; set; }
    }
}
