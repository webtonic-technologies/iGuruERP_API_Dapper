using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;
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

        public async Task<ServiceResponse<string>> AddUpdateRoutePlan(RoutePlan routePlan)
        {
            return await _routePlanRepository.AddUpdateRoutePlan(routePlan);
        }

        public async Task<ServiceResponse<IEnumerable<RoutePlan>>> GetAllRoutePlans(GetAllRoutePlanRequest request)
        {
            return await _routePlanRepository.GetAllRoutePlans(request);
        }

        public async Task<ServiceResponse<RoutePlan>> GetRoutePlanById(int routePlanId)
        {
            return await _routePlanRepository.GetRoutePlanById(routePlanId);
        }

        public async Task<ServiceResponse<bool>> UpdateRoutePlanStatus(int routePlanId)
        {
            return await _routePlanRepository.UpdateRoutePlanStatus(routePlanId);
        }
    }
}
