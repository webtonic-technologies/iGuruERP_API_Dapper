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

        public async Task<ServiceResponse<string>> AddUpdateEmployeeStopMapping(List<EmployeeStopMapping> request)
        {
            return await _routeMappingRepository.AddUpdateEmployeeStopMapping(request);
        }
        public async Task<ServiceResponse<string>> AddUpdateRouteMapping(RouteMapping routeMapping)
        {
            return await _routeMappingRepository.AddUpdateRouteMapping(routeMapping);
        }
        public async Task<ServiceResponse<string>> AddUpdateStudentStopMapping(List<StudentStopMapping> request)
        {
            return await _routeMappingRepository.AddUpdateStudentStopMapping(request);
        }
        public async Task<ServiceResponse<IEnumerable<RouteMappingResponse>>> GetAllRouteMappings(GetAllRouteMappingRequest request)
        {
            return await _routeMappingRepository.GetAllRouteMappings(request);
        }
        public async Task<ServiceResponse<List<EmployeeStopMappingResponse>>> GetEmployeeStopMappings(int RoutePlanId)
        {
            return await _routeMappingRepository.GetEmployeeStopMappings(RoutePlanId);
        }
        public async Task<ServiceResponse<RouteMappingResponse>> GetRouteMappingById(int routeMappingId)
        {
            return await _routeMappingRepository.GetRouteMappingById(routeMappingId);
        }
        public async Task<ServiceResponse<List<StudentStopMappingResponse>>> GetStudentStopMappings(int RoutePlanId)
        {
            return await _routeMappingRepository.GetStudentStopMappings(RoutePlanId);
        }
        public async Task<ServiceResponse<string>> RemoveEmployeeStopMapping(List<EmployeeStopMapping> request)
        {
            return await _routeMappingRepository.RemoveEmployeeStopMapping(request);
        }
        public async Task<ServiceResponse<string>> RemoveStudentStopMapping(List<StudentStopMapping> request)
        {
            return await _routeMappingRepository.RemoveStudentStopMapping(request);
        }
        public async Task<ServiceResponse<bool>> UpdateRouteMappingStatus(int routeMappingId)
        {
            return await _routeMappingRepository.UpdateRouteMappingStatus(routeMappingId);
        }
    }
}
