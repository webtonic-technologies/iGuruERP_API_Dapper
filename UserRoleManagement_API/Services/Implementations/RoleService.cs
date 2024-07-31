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

        public async Task<ServiceResponse<string>> SetRolePermission(SetRolePermissionRequest request)
        {
            return await _roleRepository.SetRolePermission(request);
        }

        public async Task<ServiceResponse<List<UserRoleWithPermissionsResponse>>> GetAllUserRoles()
        {
            return await _roleRepository.GetAllUserRoles();
        }
    }
}
