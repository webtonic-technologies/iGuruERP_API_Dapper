namespace Admission_API.Models
{
    public class LeadSource
    {
        public int LeadSourceID { get; set; }
        public string SourceName { get; set; }  // Renamed to avoid conflict
        public bool IsActive { get; set; }
    }
}
