namespace Transport_API.DTOs.Requests
{
    public class GetAllRoutePlanRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int InstituteId { get; set; }
        public string SearchTerm { get; set; } = string.Empty;

    }
}
