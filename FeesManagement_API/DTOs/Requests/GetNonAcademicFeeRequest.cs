namespace FeesManagement_API.DTOs.Requests
{
    public class GetNonAcademicFeeRequest
    {
        public int InstituteID { get; set; }
        public int PayeeTypeID { get; set; }
        public string StartDate { get; set; } // Change to string
        public string EndDate { get; set; }   // Change to string
        public string Search { get; set; }
    }
}
