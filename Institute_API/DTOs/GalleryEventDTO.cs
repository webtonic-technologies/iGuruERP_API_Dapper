namespace Institute_API.DTOs
{
    public class GalleryEventDTO
    {
        public int Event_id { get; set; }
        public List<string> FileNames { get; set; }
        public bool IsApproved { get; set; }

        public List<string> Base64Files { get; set; }

    }
}
