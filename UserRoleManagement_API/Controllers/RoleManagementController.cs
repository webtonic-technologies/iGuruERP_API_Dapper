using Microsoft.AspNetCore.Mvc;
using UserRoleManagement_API.DTOs.Requests;
using UserRoleManagement_API.Services.Interfaces;

namespace UserRoleManagement_API.Controllers
{
    [Route("iGuru/RoleManagement/[controller]")]
    [ApiController]
    public class RoleManagementController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleManagementController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost("SetRolePermission")]
        public async Task<IActionResult> SetRolePermission(SetRolePermissionRequest request)
        {
            var response = await _roleService.SetRolePermission(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetAllUserRoles")]
        public async Task<IActionResult> GetAllUserRoles()
        {
            var response = await _roleService.GetAllUserRoles();
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
