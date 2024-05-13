namespace Institute_API.DTOs
{
    public class ToggleGalleryActiveStatusRequest
    {
        public int GalleryId { get; set; }
        public bool isApproved { get; set; }
        public int UserId { get; set; }
    }
}
