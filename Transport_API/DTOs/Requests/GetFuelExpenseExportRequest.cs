namespace Transport_API.DTOs.Requests
{
    public class GetFuelExpenseExportRequest
    {
        public string StartDate { get; set; }  // Format: DD-MM-YYYY
        public string EndDate { get; set; }    // Format: DD-MM-YYYY
        public int VehicleID { get; set; }
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int ExportType { get; set; } // 1 for Excel, 2 for CSV
    }
}
