namespace Institute_API.DTOs
{
    public class GalleryEventDTO
    {
        public int EventId { get; set; }
        public List<string> FileNames { get; set; }
        public bool IsApproved { get; set; }

    }
}
