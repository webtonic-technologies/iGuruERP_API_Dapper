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

        [HttpGet("GetStudentStatistics/{Institute_id}")]
        public async Task<IActionResult> GetStudentStatistics(int Institute_id)
        {
            var response = await _studentService.GetStudentStatisticsAsync(Institute_id);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("HousewiseStudent/GetHouseWiseStudentCount/{Institute_id}")]
        public async Task<IActionResult> GetHouseWiseStudentCount(int Institute_id)
        {
            var response = await _studentService.GetHouseWiseStudentCountAsync(Institute_id);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("StudentBirthdays/GetTodaysBirthdays/{Institute_id}")]
        public async Task<IActionResult> GetTodaysBirthdays(int Institute_id)
        {
            var response = await _studentService.GetTodaysBirthdaysAsync(Institute_id);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GenderCount/GetClassWiseGenderCount/{Institute_id}")]
        public async Task<IActionResult> GetClassWiseGenderCount(int Institute_id)
        {
            var response = await _studentService.GetClassWiseGenderCountAsync(Institute_id);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
    }
}
