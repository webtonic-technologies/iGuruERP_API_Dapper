namespace SiteAdmin_API.DTOs.Requests
{
    public class GetAllFunctionalityRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int SubModuleId { get; set; }
    }
}
