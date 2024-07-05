namespace Admission_API.DTOs.Response
{
    public class NumberSchemeResponse
    {
        public int SchemeID { get; set; }
        public int SchemeTypeID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Suffix { get; set; }
        public string Prefix { get; set; }
        public decimal StartingNumber { get; set; }
        public string Padding { get; set; }
    }
}
