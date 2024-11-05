namespace FeesManagement_API.DTOs.Requests
{
    public class PaidFeeRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int InstituteID { get; set; }
        public string StartDate { get; set; } // Format: DD-MM-YYYY
        public string EndDate { get; set; }   // Format: DD-MM-YYYY
    }
}
