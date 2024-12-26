using UserRoleManagement_API.DTOs.Response;
using UserRoleManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserRoleManagement_API.DTOs.Requests;
using UserRoleManagement_API.DTOs.Responses;

namespace UserRoleManagement_API.Services.Interfaces
{
    public interface IRoleService
    { 
        Task<ServiceResponse<CreateNewRoleResponse>> CreateNewRole(CreateNewRoleRequest request);
        Task<ServiceResponse<List<GetUserRolesResponse>>> GetUserRoles(GetUserRolesRequest request);
        Task<ServiceResponse<string>> AssignRole(AssignRoleRequest request);
        Task<ServiceResponse<string>> DeleteRole(DeleteRoleRequest request);
        Task<ServiceResponse<GetRolesPermissionsResponse>> GetRolesPermissions(GetRolesPermissionsRequest request);
        Task<ServiceResponse<string>> DeleteEmployeeFromRole(DeleteEmployeeFromRoleRequest request);

    }
}
