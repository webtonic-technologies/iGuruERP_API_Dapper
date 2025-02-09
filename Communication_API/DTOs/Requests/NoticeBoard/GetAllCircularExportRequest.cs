namespace Communication_API.DTOs.Requests.NoticeBoard
{
    public class GetAllCircularExportRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }  // Expected format: "dd-MM-yyyy"
        public string EndDate { get; set; }    // Expected format: "dd-MM-yyyy"
        public int ExportType { get; set; }    // 1 = Excel, 2 = CSV
    }
}
