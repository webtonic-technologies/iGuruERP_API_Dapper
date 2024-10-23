using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance_SE_API.Controllers
{
    [Route("iGuru/Employee/MarkAttendance")]
    [ApiController]
    public class EmployeeAttendanceController : ControllerBase
    {
        private readonly IEmployeeAttendanceService _employeeAttendanceService;

        public EmployeeAttendanceController(IEmployeeAttendanceService employeeAttendanceService)
        {
            _employeeAttendanceService = employeeAttendanceService;
        }

        [HttpPost("SetAttendance")] // Updated route
        public async Task<IActionResult> SetAttendance([FromBody] EmployeeSetAttendanceRequest request) // Updated method name
        {
            if (request.AttendanceRecords == null || !request.AttendanceRecords.Any())
            {
                return BadRequest("Attendance records are required.");
            }

            var response = await _employeeAttendanceService.SetAttendance(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAttendance_EMP")] // Updated route
        public async Task<IActionResult> GetAttendance_EMP([FromBody] GetEmployeeAttendanceRequest request) // Updated method name
        {
            var response = await _employeeAttendanceService.GetAttendance_EMP(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
