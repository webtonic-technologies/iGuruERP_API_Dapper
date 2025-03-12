namespace StudentManagement_API.DTOs.Responses
{
    public class GetClassPromotionHistoryExportResponse
    {
        public string UserName { get; set; }
        public string DateTime { get; set; } // e.g. "12-03-2025 at 02:00 PM"
        public string IPAddress { get; set; }
    }
}
