using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/VisitorManagement/[controller]")]
    [ApiController]
    public class VisitorManagementController : ControllerBase
    {
        private readonly IVisitorLogService _visitorLogService;
        private readonly ILogger<VisitorManagementController> _logger;

        public VisitorManagementController(IVisitorLogService visitorLogService, ILogger<VisitorManagementController> logger)
        {
            _visitorLogService = visitorLogService;
            _logger = logger;
        }

        [HttpPost("AddUpdateVisitorLog")]
        public async Task<IActionResult> AddUpdateVisitorLog([FromBody] AddUpdateVisitorLogRequest request)
        {
            _logger.LogInformation("AddUpdateVisitorLog Request Received: {@Request}", request);
            var response = await _visitorLogService.AddUpdateVisitorLog(request);
            _logger.LogInformation("AddUpdateVisitorLog Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllVisitorLogs")]
        public async Task<IActionResult> GetAllVisitorLogs([FromBody] GetAllVisitorLogsRequest request)
        {
            _logger.LogInformation("GetAllVisitorLogs Request Received: {@Request}", request);
            var response = await _visitorLogService.GetAllVisitorLogs(request);
            _logger.LogInformation("GetAllVisitorLogs Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetVisitorLog/{hostelVisitorId}")]
        public async Task<IActionResult> GetVisitorLogById(int hostelVisitorId)
        {
            _logger.LogInformation("GetVisitorLogById Request Received for HostelVisitorID: {HostelVisitorID}", hostelVisitorId);
            var response = await _visitorLogService.GetVisitorLogById(hostelVisitorId);
            _logger.LogInformation("GetVisitorLogById Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Delete/{hostelVisitorId}")]
        public async Task<IActionResult> DeleteVisitorLog(int hostelVisitorId)
        {
            _logger.LogInformation("DeleteVisitorLog Request Received for HostelVisitorID: {HostelVisitorID}", hostelVisitorId);
            var response = await _visitorLogService.DeleteVisitorLog(hostelVisitorId);
            _logger.LogInformation("DeleteVisitorLog Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }
    }
}
