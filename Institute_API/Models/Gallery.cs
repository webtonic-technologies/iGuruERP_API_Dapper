namespace Institute_API.Models
{
    public class Gallery
    {
        public int GalleryId { get; set; }
        public int EventId { get; set; }
        public string FileName { get; set; }
        public bool IsApproved { get; set; }
    }
}
