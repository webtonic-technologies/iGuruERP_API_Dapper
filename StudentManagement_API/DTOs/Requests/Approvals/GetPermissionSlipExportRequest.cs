namespace StudentManagement_API.DTOs.Requests
{
    public class GetPermissionSlipExportRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string AcademicYearCode { get; set; }
        public string Search { get; set; }
        // 1 for Excel; 2 for CSV
        public int ExportType { get; set; }
    }
}
