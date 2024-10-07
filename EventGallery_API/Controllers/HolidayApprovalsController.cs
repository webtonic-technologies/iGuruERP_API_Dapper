using EventGallery_API.DTOs.Requests.Approvals;
using EventGallery_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EventGallery_API.Controllers
{
    [Route("iGuru/Holidays")]
    [ApiController]
    public class HolidayApprovalsController : ControllerBase
    {
        private readonly IHolidayApprovalService _holidayApprovalService;

        public HolidayApprovalsController(IHolidayApprovalService holidayApprovalService)
        {
            _holidayApprovalService = holidayApprovalService;
        }

        [HttpPost("GetAllHolidaysApprovals")]
        public async Task<IActionResult> GetAllHolidaysApprovals([FromBody] GetAllHolidaysApprovalsRequest request)
        {
            var result = await _holidayApprovalService.GetAllHolidaysApprovals(request);
            return Ok(result);
        }

        [HttpPost("UpdateHolidayApprovalStatus")]
        public async Task<IActionResult> UpdateHolidayApprovalStatus([FromBody] UpdateHolidayApprovalStatusRequest request)
        {
            var isUpdated = await _holidayApprovalService.UpdateHolidayApprovalStatus(request.HolidayID, request.StatusID, request.EmployeeID);

            if (isUpdated)
            {
                return Ok(new { success = true, message = "Holiday approval status updated successfully." });
            }

            return BadRequest(new { success = false, message = "Failed to update holiday approval status." });
        }
    }
}
