namespace TimeTable_API.Models
{
    public class Group
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
