using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.Responses;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;

namespace Transport_API.Repository.Interfaces
{
    public interface IRouteMappingRepository
    {
        Task<ServiceResponse<string>> AddUpdateRouteMapping(RouteMappingRequest routeMapping);
        Task<ServiceResponse<IEnumerable<RouteMappingResponse>>> GetAllRouteMappings(GetAllRouteMappingRequest request);
        Task<ServiceResponse<RouteMappingResponse>> GetRouteMappingById(int routeMappingId);
        Task<ServiceResponse<bool>> UpdateRouteMappingStatus(int routeMappingId);
        Task<ServiceResponse<string>> AddUpdateStudentStopMapping(List<StudentStopMapping> request);
        Task<ServiceResponse<string>> AddUpdateEmployeeStopMapping(List<EmployeeStopMapping> request);
        Task<ServiceResponse<string>> RemoveEmployeeStopMapping(List<EmployeeStopMapping> request);
        Task<ServiceResponse<string>> RemoveStudentStopMapping(List<StudentStopMapping> request);
        Task<ServiceResponse<List<EmployeeStopMappingResponse>>> GetEmployeeStopMappings(int RoutePlanId);
        Task<ServiceResponse<List<StudentStopMappingResponse>>> GetStudentStopMappings(int RoutePlanId);
        Task<RouteVehicleDriverInfoResponse> GetRouteVehicleDriverInfo(int routePlanID); // Add this method
        Task<IEnumerable<GetStudentsForRouteMappingResponse>> GetStudentsForRouteMapping(int classID, int sectionID, int instituteID, string search);
        Task<IEnumerable<GetEmployeesForRouteMappingResponse>> GetEmployeesForRouteMapping(int departmentID, int designationID, int instituteID, string search);
        Task<ServiceResponse<IEnumerable<GetRouteListResponse>>> GetRouteList(int instituteID);
        Task<RouteDetailsResponseDTO1> GetRouteDetailsWithStopInfo(GetAllRouteAssignedInfoRequest request);
        Task<IEnumerable<GetTransportStaffResponse>> GetTransportStaff(int instituteID);


    }
}
