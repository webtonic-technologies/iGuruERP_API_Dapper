namespace FeesManagement_API.DTOs.Requests
{
    public class ChequeTrackingExportRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int ChequeStatusID { get; set; }
        public string Search { get; set; }
        public int ExportType { get; set; } // 1 for Excel, 2 for CSV
    }
}
