namespace Institute_API.DTOs
{
    public class GalleryDTO
    {
        public int GalleryId { get; set; }
        public int EventId { get; set; }
        public string FileName { get; set; }
        public IFormFile File { get; set; }
    }

}
