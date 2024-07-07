using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;
using Transport_API.Repository.Interfaces;
using Transport_API.Services.Interfaces;

namespace Transport_API.Services.Implementations
{
    public class RouteMappingService : IRouteMappingService
    {
        private readonly IRouteMappingRepository _routeMappingRepository;

        public RouteMappingService(IRouteMappingRepository routeMappingRepository)
        {
            _routeMappingRepository = routeMappingRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateRouteMapping(RouteMapping routeMapping)
        {
            return await _routeMappingRepository.AddUpdateRouteMapping(routeMapping);
        }

        public async Task<ServiceResponse<IEnumerable<RouteMapping>>> GetAllRouteMappings(GetAllRouteMappingRequest request)
        {
            return await _routeMappingRepository.GetAllRouteMappings(request);
        }

        public async Task<ServiceResponse<RouteMapping>> GetRouteMappingById(int routeMappingId)
        {
            return await _routeMappingRepository.GetRouteMappingById(routeMappingId);
        }

        public async Task<ServiceResponse<bool>> UpdateRouteMappingStatus(int routeMappingId)
        {
            return await _routeMappingRepository.UpdateRouteMappingStatus(routeMappingId);
        }
    }
}
