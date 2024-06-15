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
    }
}
