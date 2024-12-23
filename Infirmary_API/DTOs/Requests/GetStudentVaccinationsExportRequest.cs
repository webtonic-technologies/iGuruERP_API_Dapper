namespace Infirmary_API.DTOs.Requests
{
    public class GetStudentVaccinationsExportRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int VaccinationID { get; set; }
        public int ExportType { get; set; }  // 1 for Excel, 2 for CSV
    }
}
