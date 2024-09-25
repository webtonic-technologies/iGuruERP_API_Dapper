namespace EventGallery_API.DTOs.Responses
{
    public class GalleryImageResponse
    {
        public int GalleryID { get; set; }
        public string ImageName { get; set; }
        public byte[] ImageData { get; set; }
    }
}
