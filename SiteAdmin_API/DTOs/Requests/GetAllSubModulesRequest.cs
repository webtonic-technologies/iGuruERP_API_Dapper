namespace SiteAdmin_API.DTOs.Requests
{
    public class GetAllSubModulesRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int ModuleId { get; set; }
    }
}
