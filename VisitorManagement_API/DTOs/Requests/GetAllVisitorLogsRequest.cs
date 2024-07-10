namespace VisitorManagement_API.DTOs.Requests
{
    public class GetAllVisitorLogsRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int InstituteId { get; set; }
    }
}
