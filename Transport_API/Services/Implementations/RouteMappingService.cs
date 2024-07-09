using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
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

        public Task<ServiceResponse<string>> AddUpdateEmployeeStopMapping(List<EmployeeStopMapping> request)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> AddUpdateRouteMapping(RouteMapping routeMapping)
        {
            return await _routeMappingRepository.AddUpdateRouteMapping(routeMapping);
        }

        public Task<ServiceResponse<string>> AddUpdateStudentStopMapping(List<StudentStopMapping> request)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<IEnumerable<RouteMappingResponse>>> GetAllRouteMappings(GetAllRouteMappingRequest request)
        {
            return await _routeMappingRepository.GetAllRouteMappings(request);
        }

        public Task<ServiceResponse<List<EmployeeStopMappingResponse>>> GetEmployeeStopMappings(int RoutePlanId)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<RouteMappingResponse>> GetRouteMappingById(int routeMappingId)
        {
            return await _routeMappingRepository.GetRouteMappingById(routeMappingId);
        }

        public Task<ServiceResponse<List<StudentStopMappingResponse>>> GetStudentStopMappings(int RoutePlanId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> RemoveEmployeeStopMapping(List<EmployeeStopMapping> request)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> RemoveStudentStopMapping(List<StudentStopMapping> request)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<bool>> UpdateRouteMappingStatus(int routeMappingId)
        {
            return await _routeMappingRepository.UpdateRouteMappingStatus(routeMappingId);
        }
    }
}
