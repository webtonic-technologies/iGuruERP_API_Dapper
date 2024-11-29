using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.Responses;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Repository.Interfaces;
using Transport_API.Services.Interfaces;

namespace Transport_API.Services.Implementations
{
    public class RoutePlanService : IRoutePlanService
    {
        private readonly IRoutePlanRepository _routePlanRepository;

        public RoutePlanService(IRoutePlanRepository routePlanRepository)
        {
            _routePlanRepository = routePlanRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateRoutePlan(RoutePlanRequestDTO routePlan)
        {
            return await _routePlanRepository.AddUpdateRoutePlan(routePlan);
        }

        public async Task<ServiceResponse<IEnumerable<RoutePlanResponseDTO>>> GetAllRoutePlans(GetAllRoutePlanRequest request)
        {
            return await _routePlanRepository.GetAllRoutePlans(request);
        }

        public async Task<ServiceResponse<RoutePlanResponseDTO>> GetRoutePlanById(int routePlanId)
        {
            return await _routePlanRepository.GetRoutePlanById(routePlanId);
        }

        public async Task<ServiceResponse<bool>> UpdateRoutePlanStatus(int routePlanId)
        {
            return await _routePlanRepository.UpdateRoutePlanStatus(routePlanId);
        }
        public async Task<ServiceResponse<RouteDetailsResponseDTO>> GetRouteDetails(GetRouteDetailsRequest request)
        {
            return await _routePlanRepository.GetRouteDetails(request);
        }
        public async Task<ServiceResponse<byte[]>> GetRouteDetailsExportExcel(GetRouteDetailsRequest request)
        {
            return await _routePlanRepository.GetRouteDetailsExportExcel(request);
        }
         
        public async Task<ServiceResponse<IEnumerable<GetRoutePlanVehiclesResponse>>> GetRoutePlanVehicles(int instituteID)
        {
            return await _routePlanRepository.GetRoutePlanVehicles(instituteID);
        }


    }
}
