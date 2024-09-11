using Attendance_API.DTOs;
using Attendance_API.Services.Implementations;
using Attendance_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Attendance_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class StudentAttendanceReportController : ControllerBase
    {
        private readonly IStudentAttendanceReportService _studentAttendanceReportService;
        public StudentAttendanceReportController(IStudentAttendanceReportService studentAttendanceReportService)
        {
            _studentAttendanceReportService = studentAttendanceReportService;
        }

        [HttpPost("GetStudentAttendanceDatewiseReport")]
        public async Task<IActionResult> GetStudentAttendanceDatewiseReport(StudentAttendanceDatewiseReportRequestDTO request)
        {
            var data = await _studentAttendanceReportService.GetStudentAttendanceDatewiseReport(request);
            return Ok(data);
        }

        [HttpPost("GetStudentSubjectwiseReport")]
        public async Task<IActionResult> GetStudentSubjectwiseReport(SubjectwiseAttendanceReportRequest request)
        {
            var data = await _studentAttendanceReportService.GetStudentSubjectwiseReport(request);
            return Ok(data);
        }

        [HttpPost("ExportStudentAttendanceDatewiseReportToExcel")]
        public async Task<IActionResult> ExportStudentAttendanceDatewiseReportToExcel(StudentAttendanceDatewiseReportRequestDTO request)
        {
            var response = await _studentAttendanceReportService.ExportStudentAttendanceDatewiseReportToExcel(request);

            if (response.Success)
            {
                var filePath = response.Data;
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                // Return the Excel file for download
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(filePath));
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPost("ExportStudentSubjectwiseReportToExcel")]
        public async Task<IActionResult> ExportStudentSubjectwiseReportToExcel(SubjectwiseAttendanceReportRequest request)
        {

            var response = await _studentAttendanceReportService.ExportStudentSubjectwiseReportToExcel(request);

            if (response.Success)
            {
                var filePath = response.Data;
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                // Return the Excel file for download
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(filePath));
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
    }
}
