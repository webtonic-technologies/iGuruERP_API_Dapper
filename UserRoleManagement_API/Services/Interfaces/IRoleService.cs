using UserRoleManagement_API.DTOs.Response;
using UserRoleManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserRoleManagement_API.DTOs.Requests;

namespace UserRoleManagement_API.Services.Interfaces
{
    public interface IRoleService
    {
        Task<ServiceResponse<string>> SetRolePermission(SetRolePermissionRequest request);
        Task<ServiceResponse<List<UserRoleWithPermissionsResponse>>> GetAllUserRoles();
    }
}
