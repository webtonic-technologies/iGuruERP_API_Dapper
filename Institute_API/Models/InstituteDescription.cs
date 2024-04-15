namespace Institute_API.Models
{
    public class InstituteDescription
    {
        public int Institute_description_id { get; set; }
        public int Institute_id { get; set; }
        public string Introduction { get; set; } = string.Empty;
        public string Mission_Statement { get; set; } = string.Empty;
        public string Vision { get; set; } = string.Empty;
    }
}
