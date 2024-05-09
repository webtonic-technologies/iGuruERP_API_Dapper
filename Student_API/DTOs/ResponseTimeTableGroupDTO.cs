namespace Student_API.DTOs
{
    public class ResponseTimeTableGroupDTO
    {
        public int TimetableGroupId { get; set; }
        public string GroupName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int NumberOfPeriods { get; set; }
        public int NumberOfBreaks { get; set; }
    }
}
