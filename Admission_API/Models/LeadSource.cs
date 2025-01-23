namespace Admission_API.Models
{
    public class LeadSource
    {
        public int LeadSourceID { get; set; }
        public string SourceName { get; set; }  // Renamed to avoid conflict
        public bool IsActive { get; set; }
        public int InstituteID { get; set; }
    }


    public class LeadSourceResponse
    {
        public int LeadSourceID { get; set; }
        public string SourceName { get; set; }  // Renamed to avoid conflict
        public bool IsActive { get; set; } 
    }
}
