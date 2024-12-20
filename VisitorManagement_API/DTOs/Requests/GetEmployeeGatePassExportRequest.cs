namespace VisitorManagement_API.DTOs.Requests
{
    public class GetEmployeeGatePassExportRequest
    {
        public int InstituteId { get; set; }
        public int EmployeeId { get; set; }
        public string StartDate { get; set; } = string.Empty;  // DD-MM-YYY Format
        public string EndDate { get; set; } = string.Empty;    // DD-MM-YYY Format
        public string SearchText { get; set; } = string.Empty;
        public int ExportType { get; set; } // 1 for Excel, 2 for CSV
    }
}
