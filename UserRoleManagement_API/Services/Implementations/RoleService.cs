using UserRoleManagement_API.DTOs.Response;
using UserRoleManagement_API.DTOs.ServiceResponse;
using UserRoleManagement_API.Repository.Interfaces;
using UserRoleManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserRoleManagement_API.DTOs.Requests;

namespace UserRoleManagement_API.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
         
        public async Task<ServiceResponse<CreateNewRoleResponse>> CreateNewRole(CreateNewRoleRequest request)
        {
            return await _roleRepository.CreateNewRole(request);
        }

        public async Task<ServiceResponse<List<GetUserRolesResponse>>> GetUserRoles(GetUserRolesRequest request)
        {
            return await _roleRepository.GetUserRoles(request);
        }

        public async Task<ServiceResponse<string>> AssignRole(AssignRoleRequest request)
        {
            return await _roleRepository.AssignRole(request);
        }
    }
}
