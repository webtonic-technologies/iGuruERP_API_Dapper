using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Attendance_SE_API.Controllers
{
    [Route("iGuru/Student/MarkAttendance")]
    [ApiController]
    public class MarkAttendanceController : ControllerBase
    {
        private readonly IMarkAttendanceService _markAttendanceService;

        public MarkAttendanceController(IMarkAttendanceService markAttendanceService)
        {
            _markAttendanceService = markAttendanceService;
        }

        // Endpoint to get Attendance Types
        [HttpPost("GetAttendanceType")]
        public async Task<IActionResult> GetAttendanceType()
        {
            var response = await _markAttendanceService.GetAttendanceType();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetTimeSlotType")]
        public async Task<IActionResult> GetTimeSlotType()
        {
            var response = await _markAttendanceService.GetTimeSlotType();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAttendanceSubjects")]
        public async Task<IActionResult> GetAttendanceSubjects([FromBody] AttendanceSubjectsRequest request)
        {
            var response = await _markAttendanceService.GetAttendanceSubjects(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
