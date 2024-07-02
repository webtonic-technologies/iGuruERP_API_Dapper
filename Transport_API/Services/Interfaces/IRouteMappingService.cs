using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;

namespace Transport_API.Services.Interfaces
{
    public interface IRouteMappingService
    {
        Task<ServiceResponse<string>> AddUpdateRouteMapping(RouteMapping routeMapping);
        Task<ServiceResponse<IEnumerable<RouteMapping>>> GetAllRouteMappings(GetAllRouteMappingRequest request);
        Task<ServiceResponse<RouteMapping>> GetRouteMappingById(int routeMappingId);
        Task<ServiceResponse<bool>> UpdateRouteMappingStatus(int routeMappingId);
    }
}
