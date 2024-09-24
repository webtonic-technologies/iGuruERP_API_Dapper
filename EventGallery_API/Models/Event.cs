namespace EventGallery_API.Models
{
    public class Event
    {
        public int EventID { get; set; }
        public string EventName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public TimeSpan? ScheduleTime { get; set; }
        public int AcademicYearID { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public int InstituteID { get; set; }
    }
}
