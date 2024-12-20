namespace VisitorManagement_API.DTOs.Requests
{
    public class GetAllVisitorLogsRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int InstituteId { get; set; }
        public string StartDate { get; set; } = string.Empty;  // Changed to string
        public string EndDate { get; set; } = string.Empty;    // Changed to string
        public string SearchText { get; set; } = string.Empty;
    }
}
