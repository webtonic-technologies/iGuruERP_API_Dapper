using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.ServiceResponse;

namespace Transport_API.Services.Interfaces
{
    public interface IRouteMappingService
    {
        Task<ServiceResponse<string>> AddUpdateRouteMapping(RouteMapping routeMapping);
        Task<ServiceResponse<IEnumerable<RouteMappingResponse>>> GetAllRouteMappings(GetAllRouteMappingRequest request);
        Task<ServiceResponse<RouteMappingResponse>> GetRouteMappingById(int routeMappingId);
        Task<ServiceResponse<bool>> UpdateRouteMappingStatus(int routeMappingId);
        Task<ServiceResponse<string>> AddUpdateStudentStopMapping(List<StudentStopMapping> request);
        Task<ServiceResponse<string>> AddUpdateEmployeeStopMapping(List<EmployeeStopMapping> request);
        Task<ServiceResponse<string>> RemoveEmployeeStopMapping(List<EmployeeStopMapping> request);
        Task<ServiceResponse<string>> RemoveStudentStopMapping(List<StudentStopMapping> request);
        Task<ServiceResponse<List<EmployeeStopMappingResponse>>> GetEmployeeStopMappings(int RoutePlanId);
        Task<ServiceResponse<List<StudentStopMappingResponse>>> GetStudentStopMappings(int RoutePlanId);
    }
}
