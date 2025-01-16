namespace Communication_API.DTOs.Requests.DigitalDiary
{
    public class GetAllDiaryExportRequest
    {
        public int InstituteID { get; set; }
        public int EmployeeID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Search { get; set; }
        public int ExportType { get; set; }  // 1 for Excel, 2 for CSV
    }
}
