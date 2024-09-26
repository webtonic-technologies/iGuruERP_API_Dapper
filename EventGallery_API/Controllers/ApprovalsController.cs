using EventGallery_API.DTOs.Requests;
using EventGallery_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EventGallery_API.Controllers
{
    [Route("iGuru/Approvals/[controller]")]
    [ApiController]
    public class ApprovalsController : ControllerBase
    {
        private readonly IEventApprovalService _eventApprovalService;

        public ApprovalsController(IEventApprovalService eventApprovalService)
        {
            _eventApprovalService = eventApprovalService;
        }

        [HttpPost("GetAllEventsApprovals")]
        public async Task<IActionResult> GetAllEventsApprovals([FromBody] GetAllEventsApprovalsRequest request)
        {
            var response = await _eventApprovalService.GetAllEventsApprovals(request);
            return Ok(response);
        }



        [HttpPost("UpdateEventApprovalStatus")]
        public async Task<IActionResult> UpdateEventApprovalStatus([FromBody] UpdateEventApprovalStatusRequest request)
        {
            var response = await _eventApprovalService.UpdateEventApprovalStatus(request);
            return Ok(response);
        }

    }
}
