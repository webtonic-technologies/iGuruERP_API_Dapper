using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Services.Interfaces; // Ensure this is correctly referenced
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance_SE_API.Controllers
{
    [Route("iGuru/Employee/Configuration")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeAttendanceStatusService _attendanceStatusService; 

        public EmployeeController(IEmployeeAttendanceStatusService attendanceStatusService) // Add parameter
        {
            _attendanceStatusService = attendanceStatusService; 
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

    }
}
