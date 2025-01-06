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
            var response = await _attendanceDashboardService.GetStudentAttendanceDashboard(request.InstituteID, request.AcademicYearCode, request.StartDate, request.EndDate);

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

        [HttpPost("GetAttendanceNotMarked")]
        public async Task<IActionResult> GetAttendanceNotMarked([FromBody] GetAttendanceNotMarkedRequest request)
        {
            var response = await _attendanceDashboardService.GetAttendanceNotMarked(request.InstituteID);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }


        [HttpPost("GetAbsentStudents")]
        public async Task<IActionResult> GetAbsentStudents([FromBody] GetAbsentStudentsRequest request)
        {
            var response = await _attendanceDashboardService.GetAbsentStudents(request.InstituteID);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }


        [HttpPost("GetStudentsMLCount")]
        public async Task<IActionResult> GetStudentsMLCount([FromBody] GetStudentsMLCountRequest request)
        {
            var response = await _attendanceDashboardService.GetStudentsMLCount(request.InstituteID);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }

        [HttpPost("GetHalfDayLeaveCount")]
        public async Task<IActionResult> GetHalfDayLeaveCount([FromBody] GetHalfDayLeaveCountRequest request)
        {
            var response = await _attendanceDashboardService.GetHalfDayLeaveCount(request.InstituteID);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }

        [HttpPost("GetAttendanceNotMarkedExport")]
        public async Task<IActionResult> GetAttendanceNotMarkedExport([FromBody] GetAttendanceNotMarkedExportRequest request)
        {
            var response = await _attendanceDashboardService.GetAttendanceNotMarkedExport(request);

            if (response.Success)
            {
                return File(response.Data, response.StatusCode == 200 ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv", "AttendanceNotMarkedExport" + (request.ExportType == 1 ? ".xlsx" : ".csv"));
            }

            return BadRequest(response);
        }

        [HttpPost("GetAbsentStudentsExport")]
        public async Task<IActionResult> GetAbsentStudentsExport([FromBody] GetAbsentStudentsExportRequest request)
        {
            var response = await _attendanceDashboardService.GetAbsentStudentsExport(request);

            if (response.Success)
            {
                return File(response.Data, response.StatusCode == 200 ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv",
                            "AbsentStudentsExport" + (request.ExportType == 1 ? ".xlsx" : ".csv"));
            }

            return BadRequest(response);
        }
    }
}
