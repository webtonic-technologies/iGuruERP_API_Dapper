namespace EventGallery_API.Models
{
    public class EventDetails
    {
        public int EventID { get; set; }
        public string EventName { get; set; }
        public string EventDate { get; set; } 
        public DateTime ScheduleDate { get; set; }
        public TimeSpan ScheduleTime { get; set; }
    }
}
