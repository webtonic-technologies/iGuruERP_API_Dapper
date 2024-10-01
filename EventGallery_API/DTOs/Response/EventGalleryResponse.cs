namespace EventGallery_API.DTOs.Responses
{
    public class EventGalleryResponse
    {
        public int EventID { get; set; }
        public string EventName { get; set; }
        public string EventDate { get; set; }
        public List<GalleryImageResponse> GalleryImages { get; set; }
    }
     

    public class EventDetails
    {
        public int EventID { get; set; }
        public string EventName { get; set; }
        public string EventDate { get; set; }
    }
}
