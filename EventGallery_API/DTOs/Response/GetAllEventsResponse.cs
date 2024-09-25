namespace EventGallery_API.DTOs.Responses
{
    public class GetAllEventsResponse
    {
        public int EventID { get; set; }
        public string EventName { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string Document { get; set; }
        public string Location { get; set; }
        public string EventNotification { get; set; }
        public string CreatedBy { get; set; }
    }
}
