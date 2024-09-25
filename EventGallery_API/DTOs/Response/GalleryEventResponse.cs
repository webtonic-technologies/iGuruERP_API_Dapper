using System.Collections.Generic;

namespace EventGallery_API.DTOs.Responses
{
    public class GalleryEventResponse
    {
        public int EventID { get; set; }
        public string EventName { get; set; }
        public string EventDate { get; set; }
        public List<GalleryImageResponse> Images { get; set; }
    }
}
