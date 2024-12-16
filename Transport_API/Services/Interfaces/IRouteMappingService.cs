using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.Responses;
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
        Task<ServiceResponse<RouteVehicleDriverInfoResponse>> GetRouteVehicleDriverInfo(RouteVehicleDriverInfoRequest request); // Correct the method signature
        Task<ServiceResponse<IEnumerable<GetStudentsForRouteMappingResponse>>> GetStudentsForRouteMapping(GetStudentsForRouteMappingRequest request);
        Task<ServiceResponse<IEnumerable<GetEmployeesForRouteMappingResponse>>> GetEmployeesForRouteMapping(GetEmployeesForRouteMappingRequest request);
        Task<ServiceResponse<IEnumerable<GetRouteListResponse>>> GetRouteList(int instituteID);
        Task<ServiceResponse<GetAllRouteAssignedInfoResponse>> GetAllRouteAssignedInfo(GetAllRouteAssignedInfoRequest request);

    }
}
