namespace Infirmary_API.DTOs.Requests
{
    public class GetAllStockEntriesRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string StartDate { get; set; } // Changed to string
        public string EndDate { get; set; }   // Changed to string
        public string SearchTerm { get; set; } 
    }
}
