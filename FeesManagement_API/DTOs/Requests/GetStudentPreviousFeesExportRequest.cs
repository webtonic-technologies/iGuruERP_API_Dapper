namespace FeesManagement_API.DTOs.Requests
{
    public class GetStudentPreviousFeesExportRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string AcademicYearCode { get; set; }
        public string Search { get; set; }  
        public int ExportType { get; set; }
    }
}
