namespace VisitorManagement_API.DTOs.Requests
{
    public class GetAllEmployeeGatePassRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int InstituteId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set;}
        public string SearchText { get; set; } = string.Empty;
    }
}
