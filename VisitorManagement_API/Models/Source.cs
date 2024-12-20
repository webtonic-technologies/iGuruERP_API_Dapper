namespace VisitorManagement_API.Models
{
    public class Sources
    {
        public int SourceID { get; set; }
        public string Source { get; set; }  // Renamed to SourceName
        public string Description { get; set; }
        public bool Status {  get; set; }
        public int InstituteID { get; set; }
    }
}
