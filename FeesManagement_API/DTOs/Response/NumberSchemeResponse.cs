namespace FeesManagement_API.DTOs.Responses
{
    public class NumberSchemeResponse
    {
        public int NumberSchemeID { get; set; }
        public int SchemeTypeID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public int StartingNumber { get; set; }
        public string Padding { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }  // Assuming this is part of the response
    }
}
