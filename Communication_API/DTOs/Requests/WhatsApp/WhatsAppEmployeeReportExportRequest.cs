namespace Communication_API.DTOs.Requests.WhatsApp
{
    public class WhatsAppEmployeeReportExportRequest
    {
        public string StartDate { get; set; }  // StartDate in 'DD-MM-YYYY' format
        public string EndDate { get; set; }    // EndDate in 'DD-MM-YYYY' format
        public string Search { get; set; }     // Search for Employee Name
        public int InstituteID { get; set; }   // Institute ID
        public int ExportType { get; set; }    // 1 for Excel, 2 for CSV
    }
}
