namespace Communication_API.DTOs.Requests.PushNotification
{
    public class GetNotificationEmployeeReportExportRequest
    {
        public string StartDate { get; set; }  // StartDate as string in 'DD-MM-YYYY' format
        public string EndDate { get; set; }    // EndDate as string in 'DD-MM-YYYY' format
        public int InstituteID { get; set; }
        public string Search { get; set; }  // Search term for filtering
        public int ExportType { get; set; } // 1 for Excel, 2 for CSV 
    }
}
