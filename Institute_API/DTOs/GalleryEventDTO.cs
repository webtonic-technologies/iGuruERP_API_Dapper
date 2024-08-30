using System.Text.Json.Serialization;

namespace Institute_API.DTOs
{
    public class GalleryEventDTO
    {
        public int Event_id { get; set; }
        [JsonIgnore]
        public List<string> FileNames { get; set; }
        public bool IsApproved { get; set; }
        public string EventDateTime { get; set; }   
        public int Status {  get; set; }    

        public List<string> FileName { get; set; } = new List<string>();        

    }
}
