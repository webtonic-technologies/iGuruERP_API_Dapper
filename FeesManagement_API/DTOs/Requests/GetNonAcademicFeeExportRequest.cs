namespace FeesManagement_API.DTOs.Requests
{
    public class GetNonAcademicFeeExportRequest
    {
        public int InstituteID { get; set; }
        public int PayeeTypeID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Search { get; set; }
        public int ExportType { get; set; } // 1 for Excel, 2 for CSV
    }
}
