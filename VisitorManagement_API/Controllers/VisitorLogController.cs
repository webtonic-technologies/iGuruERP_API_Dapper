using Microsoft.AspNetCore.Mvc;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.Services.Interfaces;

namespace VisitorManagement_API.Controllers
{
    [Route("iGuru/VisitorLogs")]
    [ApiController]
    public class VisitorLogController : ControllerBase
    {
        private readonly IVisitorLogService _visitorLogService;

        public VisitorLogController(IVisitorLogService visitorLogService)
        {
            _visitorLogService = visitorLogService;
        }

        [HttpPost("VisitorLogs/AddUpdateVisitorLogs")]
        public async Task<IActionResult> AddUpdateVisitorLog(VisitorRequestDTO visitorLog)
        {
            var response = await _visitorLogService.AddUpdateVisitorLog(visitorLog);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("VisitorLogs/GetAllVisitorLogs")]
        public async Task<IActionResult> GetAllVisitorLogs(GetAllVisitorLogsRequest request)
        {
            var response = await _visitorLogService.GetAllVisitorLogs(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("VisitorLogs/GetVisitorLogs/{visitorId}")]
        public async Task<IActionResult> GetVisitorLogById(int visitorId)
        {
            var response = await _visitorLogService.GetVisitorLogById(visitorId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("VisitorLogs/Status/{visitorId}")]
        public async Task<IActionResult> UpdateVisitorLogStatus(int visitorId)
        {
            var response = await _visitorLogService.UpdateVisitorLogStatus(visitorId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("VisitorLogs/GetSources")]
        public async Task<IActionResult> GetSources([FromQuery] GetSourcesRequest request)
        {
            var response = await _visitorLogService.GetSources(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("VisitorLogs/GetPurpose")]
        public async Task<IActionResult> GetPurpose([FromQuery] GetPurposeRequest request)
        {
            var response = await _visitorLogService.GetPurpose(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("VisitorLogs/GetIDProof")]
        public async Task<IActionResult> GetIDProof()
        {
            var response = await _visitorLogService.GetIDProof();
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("VisitorLogs/GetApprovalType")]
        public async Task<IActionResult> GetApprovalType()
        {
            var response = await _visitorLogService.GetApprovalType();
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("VisitorLogs/GetEmployee")]
        public async Task<IActionResult> GetEmployee([FromQuery] GetEmployeeRequest request)
        {
            var response = await _visitorLogService.GetEmployee(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("VisitorLogs/GetVisitorSlip")]
        public async Task<IActionResult> GetVisitorSlip([FromQuery] GetVisitorSlipRequest request)
        {
            var response = await _visitorLogService.GetVisitorSlip(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("VisitorLogs/ChangeApprovalStatus")]
        public async Task<IActionResult> ChangeApprovalStatus([FromQuery] ChangeApprovalStatusRequest request)
        {
            var response = await _visitorLogService.ChangeApprovalStatus(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
