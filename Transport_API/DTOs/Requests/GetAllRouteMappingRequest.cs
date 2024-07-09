namespace Transport_API.DTOs.Requests
{
    public class GetAllRouteMappingRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int RoutePlanId { get; set; }
    }
}
