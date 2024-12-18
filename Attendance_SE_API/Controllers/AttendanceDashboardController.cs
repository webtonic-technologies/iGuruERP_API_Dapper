using Attendance_SE_API.Services.Interfaces;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;

namespace Attendance_SE_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class AttendanceDashboardController : ControllerBase
    {
        private readonly IAttendanceDashboardService _attendanceDashboardService;

        public AttendanceDashboardController(IAttendanceDashboardService attendanceDashboardService)
        {
            _attendanceDashboardService = attendanceDashboardService;
        }

        [HttpPost("GetStudentAttendanceStatistics")]
        public async Task<IActionResult> GetStudentAttendanceStatistics([FromBody] AttendanceStatisticsRequest request)
        {
            var response = await _attendanceDashboardService.GetStudentAttendanceStatistics(request.InstituteID, request.AcademicYearCode);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }

        [HttpPost("GetStudentAttendanceDashboard")]
        public async Task<IActionResult> GetStudentAttendanceDashboard([FromBody] GetStudentAttendanceDashboardRequest request)
        {
            var response = await _attendanceDashboardService.GetStudentAttendanceDashboard(request.InstituteID, request.AcademicYearCode);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }

        [HttpPost("GetEmployeeAttendanceStatistics")]
        public async Task<IActionResult> GetEmployeeAttendanceStatistics([FromBody] GetEmployeeAttendanceStatisticsRequest request)
        {
            var response = await _attendanceDashboardService.GetEmployeeAttendanceStatistics(request.InstituteID);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }

        [HttpPost("GetEmployeeOnLeave")]
        public async Task<IActionResult> GetEmployeeOnLeave([FromBody] GetEmployeeOnLeaveRequest request)
        {
            var response = await _attendanceDashboardService.GetEmployeeOnLeave(request.InstituteID);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }
    }
}
