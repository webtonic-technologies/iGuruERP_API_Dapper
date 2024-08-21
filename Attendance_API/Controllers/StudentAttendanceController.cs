using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Attendance_API.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Attendance_API.DTOs;

namespace Attendance_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class StudentAttendanceController : ControllerBase
    {
        private readonly IStudentAttendanceMasterService _studentAttendanceMasterService;

        public StudentAttendanceController(IStudentAttendanceMasterService studentAttendanceMasterService)
        {
            _studentAttendanceMasterService = studentAttendanceMasterService;
        }

        [HttpPost("GetStudentAttendanceMasterList")]
        public async Task<IActionResult> GetStudentAttendanceMasterList([FromBody] StudentAttendanceMasterRequestDTO request)
        {
            var data = await _studentAttendanceMasterService.GetStudentAttendanceMasterList(request);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdateStudentAttendanceMasters([FromBody] List<StudentAttendanceMasterDTO> studentAttendanceMasters)
        {
            var data = await _studentAttendanceMasterService.InsertOrUpdateStudentAttendanceMasters(studentAttendanceMasters);
            return Ok(data);
        }

        [HttpDelete("{studentAttendanceId}")]
        public async Task<IActionResult> DeleteStudentAttendanceMaster(int studentAttendanceId)
        {
            var data = await _studentAttendanceMasterService.DeleteStudentAttendanceMaster(studentAttendanceId);
            return Ok(data);
        }

        [HttpGet("GetTimeSlotsForDropdown")]
        public async Task<IActionResult> GetTimeSlotsForDropdown()
        {
            var data = await _studentAttendanceMasterService.GetTimeSlotsForDropdown();
            return Ok(data);
        }
    }
}
