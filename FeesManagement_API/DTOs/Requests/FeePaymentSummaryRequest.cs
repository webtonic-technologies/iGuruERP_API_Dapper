namespace FeesManagement_API.DTOs.Requests
{
    public class FeePaymentSummaryRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int InstituteID { get; set; }
        public bool Active { get; set; } // If true, show students with IsActive = 1; if false, show IsActive = 0
    }
}
