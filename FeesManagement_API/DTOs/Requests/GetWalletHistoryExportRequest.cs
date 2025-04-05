namespace FeesManagement_API.DTOs.Requests
{
    public class GetWalletHistoryExportRequest
    {
        public int StudentID { get; set; }
        public int InstituteID { get; set; }
        public int ExportType { get; set; } // e.g. 1 for Excel, 2 for CSV (not used in JSON export)
    }
}
