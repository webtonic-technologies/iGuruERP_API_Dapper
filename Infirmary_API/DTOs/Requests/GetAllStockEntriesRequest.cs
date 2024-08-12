namespace Infirmary_API.DTOs.Requests
{
    public class GetAllStockEntriesRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public DateTime? StartDate { get; set; } // New property for StartDate
        public DateTime? EndDate { get; set; }   // New property for EndDate

    }
}
