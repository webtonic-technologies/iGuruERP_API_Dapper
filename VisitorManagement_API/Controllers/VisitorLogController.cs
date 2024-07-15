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
    }
}
