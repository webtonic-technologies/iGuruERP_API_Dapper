namespace Infirmary_API.DTOs.Requests
{
    public class GetStockEntriesExportRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }  // Format: "DD-MM-YYYY"
        public string EndDate { get; set; }    // Format: "DD-MM-YYYY"
        public string SearchTerm { get; set; }
        public int ExportType { get; set; }  // 1 for Excel, 2 for CSV
    }
}
