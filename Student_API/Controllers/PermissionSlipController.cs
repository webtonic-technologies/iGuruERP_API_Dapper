using Microsoft.AspNetCore.Mvc;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
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

        [HttpPost("GetAllPermissionSlips")]
        public async Task<IActionResult> GetAllPermissionSlips(GetAllPermissionSlips obj)
        {
            var response = await _permissionSlipService.GetAllPermissionSlips(obj.Institute_id, obj.classId, obj.sectionId, obj.pageNumber, obj.pageSize);
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
        [HttpPost("GetAllStudentApprovedList")]
        public async Task<IActionResult> GetAllStudentApprovedList(GetAllPermissionSlipsByStatus obj)
        {
            var response = await _permissionSlipService.GetPermissionSlips(obj.Institute_id, obj.classId, obj.sectionId, obj.startDate, obj.endDate, true, obj.pageNumber, obj.pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("GetAllStudentRejectedList")]
        public async Task<IActionResult> GetAllStudentRejectedList(GetAllPermissionSlipsByStatus obj)
        {
            var response = await _permissionSlipService.GetPermissionSlips(obj.Institute_id, obj.classId, obj.sectionId, obj.startDate, obj.endDate, false, obj.pageNumber, obj.pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

    }
}
