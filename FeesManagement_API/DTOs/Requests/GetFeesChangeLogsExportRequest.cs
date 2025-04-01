namespace FeesManagement_API.DTOs.Requests
{
    public class GetFeesChangeLogsExportRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string AcademicYearCode { get; set; }
        public int ExportType { get; set; } // 1 for Excel, 2 for CSV
    }
}
