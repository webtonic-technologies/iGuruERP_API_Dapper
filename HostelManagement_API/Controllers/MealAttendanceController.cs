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
    [Route("iGuru/MealManagement/[controller]")]
    [ApiController]
    public class MealAttendanceController : ControllerBase
    {
        private readonly IMealAttendanceService _mealAttendanceService;
        private readonly ILogger<MealAttendanceController> _logger;

        public MealAttendanceController(IMealAttendanceService mealAttendanceService, ILogger<MealAttendanceController> logger)
        {
            _mealAttendanceService = mealAttendanceService;
            _logger = logger;
        }

        [HttpPost("GetMealAttendance")]
        public async Task<IActionResult> GetMealAttendance([FromBody] GetMealAttendanceRequest request)
        {
            _logger.LogInformation("GetMealAttendance Request Received: {@Request}", request);
            var result = await _mealAttendanceService.GetMealAttendance(request);
            _logger.LogInformation("GetMealAttendance Response: {@Response}", result);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("SetMealAttendance")]
        public async Task<IActionResult> SetMealAttendance([FromBody] SetMealAttendanceRequest request)
        {
            _logger.LogInformation("SetMealAttendance Request Received: {@Request}", request);
            ServiceResponse<string> result = await _mealAttendanceService.SetMealAttendance(request);
            _logger.LogInformation("SetMealAttendance Response: {@Response}", result);
            return StatusCode(result.StatusCode, result);
        }
    }
}
