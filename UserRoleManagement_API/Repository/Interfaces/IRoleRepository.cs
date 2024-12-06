using UserRoleManagement_API.DTOs.Response;
using UserRoleManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserRoleManagement_API.DTOs.Requests;

namespace UserRoleManagement_API.Repository.Interfaces
{
    public interface IRoleRepository
    { 
        Task<ServiceResponse<CreateNewRoleResponse>> CreateNewRole(CreateNewRoleRequest request);
        Task<ServiceResponse<List<GetUserRolesResponse>>> GetUserRoles(GetUserRolesRequest request);
        Task<ServiceResponse<string>> AssignRole(AssignRoleRequest request);

    }
}
