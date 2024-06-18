using Microsoft.AspNetCore.Mvc;
using Student_API.Services.Interfaces;

namespace Student_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class StudentDashboardController : ControllerBase
    {
        private readonly IStudentDashboardService _studentService;

        public StudentDashboardController(IStudentDashboardService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet("GetStudentStatistics")]
        public async Task<IActionResult> GetStudentStatistics()
        {
            var response = await _studentService.GetStudentStatisticsAsync();
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("GetHouseWiseStudentCount")]
        public async Task<IActionResult> GetHouseWiseStudentCount()
        {
            var response = await _studentService.GetHouseWiseStudentCountAsync();
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("GetTodaysBirthdays")]
        public async Task<IActionResult> GetTodaysBirthdays()
        {
            var response = await _studentService.GetTodaysBirthdaysAsync();
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetClassWiseGenderCount")]
        public async Task<IActionResult> GetClassWiseGenderCount()
        {
            var response = await _studentService.GetClassWiseGenderCountAsync();
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
    }
}
