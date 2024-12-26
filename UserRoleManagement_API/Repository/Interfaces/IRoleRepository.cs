using UserRoleManagement_API.DTOs.Response;
using UserRoleManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserRoleManagement_API.DTOs.Requests;
using UserRoleManagement_API.DTOs.Responses;

namespace UserRoleManagement_API.Repository.Interfaces
{
    public interface IRoleRepository
    { 
        Task<ServiceResponse<CreateNewRoleResponse>> CreateNewRole(CreateNewRoleRequest request);
        Task<ServiceResponse<List<GetUserRolesResponse>>> GetUserRoles(GetUserRolesRequest request);
        Task<ServiceResponse<string>> AssignRole(AssignRoleRequest request);
        Task<bool> DeleteRoleAsync(DeleteRoleRequest request);
        Task<GetRolesPermissionsResponse> GetRolesPermissionsAsync(GetRolesPermissionsRequest request);
        Task<bool> DeleteEmployeeFromRoleAsync(DeleteEmployeeFromRoleRequest request);


    }
}
