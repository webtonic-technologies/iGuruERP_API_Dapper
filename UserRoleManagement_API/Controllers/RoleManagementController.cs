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
          
        [HttpPost("CreateNewRole")]
        public async Task<IActionResult> CreateNewRole(CreateNewRoleRequest request)
        {
            var response = await _roleService.CreateNewRole(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles([FromQuery] GetUserRolesRequest request)
        {
            var response = await _roleService.GetUserRoles(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole(AssignRoleRequest request)
        {
            var response = await _roleService.AssignRole(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
