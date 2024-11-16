using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace Attendance_SE_API.Controllers
{
    [Route("iGuru/Student/Configuration")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentAttendanceStatusService _attendanceStatusService;
        private readonly IAttendanceService _attendanceService;
        private readonly IStudentAttendanceReportService _attendanceReportService;

        public StudentController(IStudentAttendanceStatusService attendanceStatusService,
                                 IAttendanceService attendanceService,
                                 IStudentAttendanceReportService attendanceReportService)
        {
            _attendanceStatusService = attendanceStatusService;
            _attendanceService = attendanceService;
            _attendanceReportService = attendanceReportService; // Initialize attendance report service
        }

        // Attendance Status Methods
        [HttpPost("AddUpdateAttendanceStatus")]
        public async Task<IActionResult> AddUpdateAttendanceStatus([FromBody] AddUpdateAttendanceStatusRequest request)
        {
            if (request.AttendanceStatus == null || !request.AttendanceStatus.Any())
            {
                return BadRequest("Attendance status list is required.");
            }

            var response = await _attendanceStatusService.AddUpdateAttendanceStatus(request.AttendanceStatus);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllAttendanceStatus")]
        public async Task<IActionResult> GetAllAttendanceStatus([FromBody] GetAllAttendanceStatusRequest request)
        {
            var response = await _attendanceStatusService.GetAllAttendanceStatuses(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllAttendanceStatusDDL")]
        public async Task<IActionResult> GetAllAttendanceStatusDDL([FromBody] GetAllAttendanceStatusDDLRequest request)
        {
            var response = await _attendanceStatusService.GetAllAttendanceStatusesDDL(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpGet("GetAttendanceStatusByID/{StatusID}")]
        public async Task<IActionResult> GetAttendanceStatusByID(int StatusID)
        {
            var response = await _attendanceStatusService.GetAttendanceStatusById(StatusID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("DeleteStatus/{StatusID}")]
        public async Task<IActionResult> DeleteStatus(int StatusID)
        {
            var response = await _attendanceStatusService.DeleteStatus(StatusID);
            return StatusCode(response.StatusCode, response);
        }

        // Mark Attendance Methods
        [HttpPost("GetAttendance")]
        public async Task<IActionResult> GetAttendance([FromBody] GetAttendanceRequest request)
        {
            var response = await _attendanceService.GetAttendance(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("SetAttendance")]
        public async Task<IActionResult> SetAttendance([FromBody] SetAttendanceRequest request)
        {
            if (request.AttendanceRecords == null || !request.AttendanceRecords.Any())
            {
                return BadRequest("Attendance records are required.");
            }

            var response = await _attendanceService.SetAttendance(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("StudentAttendanceReport/GetAttendanceReport")]
        public async Task<IActionResult> GetAttendanceReport([FromBody] StudentAttendanceReportRequest request)
        {
            var response = await _attendanceReportService.GetAttendanceReport(request);
            return Ok(response);
        }
    }
}
