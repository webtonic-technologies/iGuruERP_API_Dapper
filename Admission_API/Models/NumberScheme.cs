namespace Admission_API.Models
{
    public class NumberScheme
    {
        public int SchemeID { get; set; }
        public int SchemeTypeID { get; set; }
        public string FromDate { get; set; }  // Changed from DateTime to string
        public string ToDate { get; set; }    // Changed from DateTime to string

        public string Suffix { get; set; }
        public string Prefix { get; set; }
        public decimal StartingNumber { get; set; }
        public string Padding { get; set; }
        public int InstituteID { get; set; }
    }

    public class NumberSchemeResponse
    {
        public int SchemeID { get; set; }
        public int SchemeTypeID { get; set; }
        public string SchemeType { get; set; } 
        public string FromDate { get; set; }  // Changed from DateTime to string
        public string ToDate { get; set; }    // Changed from DateTime to string 
        public string Suffix { get; set; }
        public string Prefix { get; set; }
        public decimal StartingNumber { get; set; }
        public string Padding { get; set; }
        public int InstituteID { get; set; }
    }

    public class SchemeType
    {
        public int SchemeTypeID { get; set; }
        public string SchemeTypeName { get; set; }
    }
}
