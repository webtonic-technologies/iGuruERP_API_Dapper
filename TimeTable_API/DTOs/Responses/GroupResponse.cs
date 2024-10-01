namespace TimeTable_API.DTOs.Responses
{
    public class GroupResponse
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string StartEndTime { get; set; } // Formatted as "08:00 AM - 12:00 PM"
        public int NumberOfSessions { get; set; }
        public int NumberOfBreaks { get; set; }
        public List<ClassSectionResponse> ClassSections { get; set; }
    }



    public class ClassSectionResponse
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }
}
