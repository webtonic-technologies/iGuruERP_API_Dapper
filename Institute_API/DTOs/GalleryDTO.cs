using System.Text.Json.Serialization;

namespace Institute_API.DTOs
{
    public class GalleryDTO
    {
        public int? GalleryId { get; set; }
        public int EventId { get; set; }
        public int Institute_id {  get; set; }      
        public List<string> FileName { get; set; }
        //public IFormFile File { get; set; }
        //public string? Base64File { get; set; }
    }

}
