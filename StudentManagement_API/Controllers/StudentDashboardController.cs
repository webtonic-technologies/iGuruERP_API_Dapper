using Microsoft.AspNetCore.Mvc;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.ServiceResponse;
using StudentManagement_API.Services.Interfaces;
using System.Threading.Tasks;

namespace StudentManagement_API.Controllers
{
    [Route("iGuru/StudentDashboard/Dashboard")]
    [ApiController]
    public class StudentDashboardController : ControllerBase
    {
        private readonly IStudentDashboardService _studentDashboardService;
        public StudentDashboardController(IStudentDashboardService studentDashboardService)
        {
            _studentDashboardService = studentDashboardService;
        }

        [HttpPost("GetStudentStatistics")]
        public async Task<IActionResult> GetStudentStatistics([FromBody] GetStudentStatisticsRequest request)
        {
            var response = await _studentDashboardService.GetStudentStatisticsAsync(request);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(response.StatusCode, response.Message);
            }
        }

        [HttpPost("GetStudentStatusStatistics")]
        public async Task<IActionResult> GetStudentStatusStatistics([FromBody] GetStudentStatusStatisticsRequest request)
        {
            var response = await _studentDashboardService.GetStudentStatusStatisticsAsync(request);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(response.StatusCode, response.Message);
            }
        }


        [HttpPost("GetStudentTypeStatistics")]
        public async Task<IActionResult> GetStudentTypeStatistics([FromBody] GetStudentTypeStatisticsRequest request)
        {
            var response = await _studentDashboardService.GetStudentTypeStatisticsAsync(request);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(response.StatusCode, response.Message);
            }
        }

        [HttpPost("GetHouseWiseStudent")]
        public async Task<IActionResult> GetHouseWiseStudent([FromBody] GetHouseWiseStudentRequest request)
        {
            var response = await _studentDashboardService.GetHouseWiseStudentAsync(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response.Message);
        }

        [HttpPost("GetStudentBirthdays")]
        public async Task<IActionResult> GetStudentBirthdays([FromBody] GetStudentBirthdaysRequest request)
        {
            var response = await _studentDashboardService.GetStudentBirthdaysAsync(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response.Message);
        }

        [HttpPost("GetClassWiseStudents")]
        public async Task<IActionResult> GetClassWiseStudents([FromBody] GetClassWiseStudentsRequest request)
        {
            var response = await _studentDashboardService.GetClassWiseStudentsAsync(request);
            if (response.Success)
                return Ok(response);
            return StatusCode(response.StatusCode, response.Message);
        }

    }
}
