namespace FeesManagement_API.DTOs.Requests
{
    public class AddUpdateNumberSchemeRequest
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
    }
}
