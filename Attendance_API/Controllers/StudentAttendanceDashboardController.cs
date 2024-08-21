using Attendance_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Attendance_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class StudentAttendanceDashboardController : ControllerBase
    {
        private readonly IStudentAttendanceDashboardService _studentAttendanceDashboardService;

        public StudentAttendanceDashboardController(IStudentAttendanceDashboardService studentAttendanceDashboardService)
        {
            _studentAttendanceDashboardService = studentAttendanceDashboardService;
        }

        [HttpGet("GetAttendanceCountsForToday")]
        public async Task<IActionResult> GetAttendanceCountsForToday(int instituteId)
        {
            var response = await _studentAttendanceDashboardService.GetAttendanceCountsForTodayAsync(instituteId);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetClasswiseAttendanceCountsForToday")]
        public async Task<IActionResult> GetClasswiseAttendanceCountsForToday(int instituteId)
        {
            var response = await _studentAttendanceDashboardService.GetClasswiseAttendanceCountsForTodayAsync(instituteId);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("GetEmployeesOnLeaveForTodayAsync")]
        public async Task<IActionResult> GetEmployeesOnLeaveForTodayAsync(int instituteId)
        {
            var response = await _studentAttendanceDashboardService.GetEmployeesOnLeaveForTodayAsync(instituteId);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("GetAbsentStudentsForTodayAsync")]
        public async Task<IActionResult> GetAbsentStudentsForTodayAsync(int instituteId)
        {
            var response = await _studentAttendanceDashboardService.GetAbsentStudentsForTodayAsync(instituteId);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
    }
}
