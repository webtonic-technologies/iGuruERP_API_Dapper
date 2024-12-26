using UserRoleManagement_API.DTOs.Response;
using UserRoleManagement_API.DTOs.ServiceResponse;
using UserRoleManagement_API.Repository.Interfaces;
using UserRoleManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserRoleManagement_API.DTOs.Requests;
using UserRoleManagement_API.DTOs.Responses;

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

        public async Task<ServiceResponse<string>> DeleteRole(DeleteRoleRequest request)
        {
            try
            {
                bool isDeleted = await _roleRepository.DeleteRoleAsync(request);

                if (isDeleted)
                {
                    return new ServiceResponse<string>(true, "Role Deactivated Successfully", "Success", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Role Deactivation Failed", null, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<GetRolesPermissionsResponse>> GetRolesPermissions(GetRolesPermissionsRequest request)
        {
            var response = await _roleRepository.GetRolesPermissionsAsync(request);

            if (response != null)
            {
                return new ServiceResponse<GetRolesPermissionsResponse>(true, "Permissions fetched successfully", response, 200);
            }

            return new ServiceResponse<GetRolesPermissionsResponse>(false, "Failed to fetch permissions", null, 500);
        }

        public async Task<ServiceResponse<string>> DeleteEmployeeFromRole(DeleteEmployeeFromRoleRequest request)
        {
            try
            {
                bool isDeleted = await _roleRepository.DeleteEmployeeFromRoleAsync(request);

                if (isDeleted)
                {
                    return new ServiceResponse<string>(true, "Employee removed from role successfully", "Success", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Failed to remove employee from role", null, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }
    }
}
