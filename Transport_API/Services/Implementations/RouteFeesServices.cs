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
        public Task<ServiceResponse<string>> AddUpdateRouteFeeStructure(RouteFeeStructure request)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<RouteFeeStructure>> GetRouteFeeStructureById(int routeFeeStructureId)
        {
            throw new NotImplementedException();
        }
    }
}
