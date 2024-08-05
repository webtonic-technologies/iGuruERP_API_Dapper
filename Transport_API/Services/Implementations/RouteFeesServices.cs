using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Repository.Interfaces;
using Transport_API.Services.Interfaces;

namespace Transport_API.Services.Implementations
{
    public class RouteFeesServices : IRouteFeesServices
    {
        private readonly IRouteFeesRepository _routeFeesRepository;

        public RouteFeesServices(IRouteFeesRepository routeFeesRepository)
        {
            _routeFeesRepository = routeFeesRepository;
        }
        public async Task<ServiceResponse<string>> AddUpdateRouteFeeStructure(RouteFeeStructure request)
        {
            return await _routeFeesRepository.AddUpdateRouteFeeStructure(request);
        }

        public async Task<ServiceResponse<RouteFeeStructure>> GetRouteFeeStructureById(int routeFeeStructureId)
        {
            return await _routeFeesRepository.GetRouteFeeStructureById(routeFeeStructureId);
        }
    }
}
