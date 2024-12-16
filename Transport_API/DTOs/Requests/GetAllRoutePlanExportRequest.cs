namespace Transport_API.DTOs.Requests
{
    public class GetAllRoutePlanExportRequest
    {
        public int InstituteId { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int ExportType { get; set; } // 1 for Excel, 2 for CSV
    }
}
