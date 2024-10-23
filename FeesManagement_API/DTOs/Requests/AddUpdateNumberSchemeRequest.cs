namespace FeesManagement_API.DTOs.Requests
{
    public class AddUpdateNumberSchemeRequest
    {
        public int NumberSchemeID { get; set; }
        public int SchemeTypeID { get; set; }
        public string FromDate { get; set; } // Changed to string to store formatted date
        public string ToDate { get; set; } // Changed to string to store formatted date
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public int StartingNumber { get; set; }
        public string Padding { get; set; }
        public int InstituteID { get; set; }
    }
}
