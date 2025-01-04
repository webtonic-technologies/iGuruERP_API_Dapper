namespace Infirmary_API.DTOs.Requests
{
    public class GetStockSummaryReportExportRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }  // Date in 'DD-MM-YYYY' format
        public string EndDate { get; set; }    // Date in 'DD-MM-YYYY' format
        public int ExportType { get; set; }    // 1 for Excel, 2 for CSV
    }
}
