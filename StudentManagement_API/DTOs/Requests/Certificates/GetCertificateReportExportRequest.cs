namespace StudentManagement_API.DTOs.Requests
{
    public class GetCertificateReportExportRequest
    {
        public int InstituteID { get; set; }
        public string AcademicYearCode { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int TemplateID { get; set; }
        public string Search { get; set; }
        public int ExportType { get; set; } // 1 = Excel, 2 = CSV
    }
}
