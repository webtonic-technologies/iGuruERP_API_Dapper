namespace Infirmary_API.DTOs.Requests
{
    public class GetStockSummaryReportRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }  // Date in 'DD-MM-YYYY' format
        public string EndDate { get; set; }    // Date in 'DD-MM-YYYY' format
        public int PageNumber { get; set; }    // Page number for pagination
        public int PageSize { get; set; }      // Number of records per page
    }
}
