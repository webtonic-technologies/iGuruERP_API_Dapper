using Microsoft.AspNetCore.Mvc;
using Student_API.DTOs;
using Student_API.Services.Interfaces;

namespace Student_API.Controllers
{
    [Route("iGuru/Approvals/[controller]")]
    [ApiController]
    public class PermissionSlipController : ControllerBase
    {
        private readonly IPermissionSlipService _permissionSlipService;

        public PermissionSlipController(IPermissionSlipService permissionSlipService)
        {
            _permissionSlipService = permissionSlipService;
        }

        [HttpGet("GetAllPermissionSlips")]
        public async Task<IActionResult> GetAllPermissionSlips(int classId, int sectionId, int? pageNumber = null, int? pageSize = null)
        {
            var response = await _permissionSlipService.GetAllPermissionSlips(classId, sectionId,pageNumber,pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("UpdatePermissionSlipStatus")]
        public async Task<IActionResult> UpdatePermissionSlipStatus([FromBody] UpdatePermissionSlipStatusRequest request)
        {
            var response = await _permissionSlipService.UpdatePermissionSlipStatus(request.PermissionSlipId, request.IsApproved);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("GetAllStudentApprovedList")]
        public async Task<IActionResult> GetAllStudentApprovedList(int classId, int sectionId, DateTime? startDate, DateTime? endDate, int? pageNumber = null, int? pageSize = null)
        {
            var response = await _permissionSlipService.GetPermissionSlips(classId, sectionId, startDate, endDate,true,pageNumber, pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("GetAllStudentRejectedList")]
        public async Task<IActionResult> GetAllStudentRejectedList(int classId, int sectionId, DateTime? startDate, DateTime? endDate, int? pageNumber = null, int? pageSize = null)
        {
            var response = await _permissionSlipService.GetPermissionSlips(classId, sectionId, startDate, endDate, false, pageNumber, pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

    }
}
