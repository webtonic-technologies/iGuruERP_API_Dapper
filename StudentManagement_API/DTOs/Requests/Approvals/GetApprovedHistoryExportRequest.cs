namespace StudentManagement_API.DTOs.Requests
{
    public class GetApprovedHistoryExportRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        // Dates as strings in "dd-MM-yyyy" format
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Search { get; set; }
        // 1 for Excel; 2 for CSV
        public int ExportType { get; set; }
    }
}
