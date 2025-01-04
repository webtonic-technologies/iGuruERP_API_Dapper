namespace Infirmary_API.DTOs.Requests
{
    public class GetStockHistoryExportRequest
    {
        public int StockID { get; set; }
        public int InstituteID { get; set; }
        public int ExportType { get; set; }    // 1 for Excel, 2 for CSV
    }
}
