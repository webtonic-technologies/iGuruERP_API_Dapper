namespace FeesManagement_API.DTOs.Responses
{
    public class NumberSchemeResponse
    {
        public int NumberSchemeID { get; set; }
        public int SchemeTypeID { get; set; }
        public string SchemeType { get; set; } // New property for SchemeType
        public DateTime FromDate { get; set; } // Added property for FromDate
        public DateTime ToDate { get; set; } // Added property for ToDate
        public string FromDateFormatted { get; set; } // Formatted date as string
        public string ToDateFormatted { get; set; } // Formatted date as string
        public string DateRange { get; set; } // Combined date range
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public int StartingNumber { get; set; }
        public string Padding { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }  // Assuming this is part of the response
    }
}
