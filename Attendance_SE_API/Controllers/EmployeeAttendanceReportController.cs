using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Attendance_SE_API.Controllers
{
    [Route("iGuru/Employee/EmployeeAttendanceReport")]
    [ApiController]
    public class EmployeeAttendanceReportController : ControllerBase
    {
        private readonly IEmployeeAttendanceReportService _employeeAttendanceReportService;

        public EmployeeAttendanceReportController(IEmployeeAttendanceReportService employeeAttendanceReportService)
        {
            _employeeAttendanceReportService = employeeAttendanceReportService;
        }

        [HttpPost("GetAttendanceReport")]
        public async Task<IActionResult> GetAttendanceReport([FromBody] EmployeeAttendanceReportRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            // Validate request properties if necessary
            if (string.IsNullOrEmpty(request.StartDate) || string.IsNullOrEmpty(request.EndDate))
            {
                return BadRequest("StartDate and EndDate are required.");
            }

            var response = await _employeeAttendanceReportService.GetAttendanceReport(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
