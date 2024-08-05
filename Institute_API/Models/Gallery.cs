namespace Institute_API.Models
{
    public class Gallery
    {
        public int GalleryId { get; set; }
        public int Event_id { get; set; }
        public string FileName { get; set; }
        public bool IsApproved { get; set; }
    }
}
