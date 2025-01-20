using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Services.Implementations;
using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/Hostel/[controller]")]
    [ApiController]
    public class HostelAttendanceController : ControllerBase
    {
        private readonly IHostelAttendanceService _hostelAttendanceService;
        private readonly ILogger<HostelAttendanceController> _logger;

        public HostelAttendanceController(IHostelAttendanceService hostelAttendanceService, ILogger<HostelAttendanceController> logger)
        {
            _hostelAttendanceService = hostelAttendanceService;
            _logger = logger;
        }

        [HttpPost("SetHostelAttendance")]
        public async Task<IActionResult> SetHostelAttendance([FromBody] SetHostelAttendanceRequest request)
        {
            _logger.LogInformation("SetHostelAttendance Request Received: {@Request}", request);
            ServiceResponse<string> result = await _hostelAttendanceService.SetHostelAttendance(request);
            _logger.LogInformation("SetHostelAttendance Response: {@Response}", result);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetHostelAttendance")]
        public async Task<IActionResult> GetHostelAttendance([FromBody] GetHostelAttendanceRequest request)
        {
            _logger.LogInformation("GetHostelAttendance Request Received: {@Request}", request);
            var result = await _hostelAttendanceService.GetHostelAttendance(request);
            _logger.LogInformation("GetHostelAttendance Response: {@Response}", result);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetHostelAttendanceType")]
        public async Task<IActionResult> GetHostelAttendanceType()
        {
            var response = await _hostelAttendanceService.GetHostelAttendanceTypes();
            return Ok(new { Success = true, Message = "Attendance types retrieved successfully", Data = response });
        }

        [HttpPost("GetHostelAttendanceStatus")]
        public async Task<IActionResult> GetHostelAttendanceStatus()
        {
            var response = await _hostelAttendanceService.GetHostelAttendanceStatuses();
            return Ok(new { Success = true, Message = "Attendance statuses retrieved successfully", Data = response });
        }
    }
}
