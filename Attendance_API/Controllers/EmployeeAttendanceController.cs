using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Attendance_API.Services.Interfaces;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using System.Threading.Tasks;
using Attendance_API.Services.Implementations;

namespace Attendance_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class EmployeeAttendanceController : ControllerBase
    {
        private readonly IEmployeeAttendanceService _employeeAttendanceService;

        public EmployeeAttendanceController(IEmployeeAttendanceService employeeAttendanceService)
        {
            _employeeAttendanceService = employeeAttendanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeAttendanceMasterList([FromQuery] EmployeeAttendanceMasterRequestDTO request)
        {
            var res = await _employeeAttendanceService.GetEmployeeAttendanceMasterList(request);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdateEmployeeAttendanceMaster(EmployeeAttendanceMasterDTO employeeAttendanceMaster)
        {
            var res = await _employeeAttendanceService.InsertOrUpdateEmployeeAttendanceMaster(employeeAttendanceMaster);
            return Ok(res);
        }

        [HttpDelete("{employeeAttendanceId}")]
        public async Task<IActionResult> DeleteEmployeeAttendanceMaster(int employeeAttendanceId)
        {
            var res = await _employeeAttendanceService.DeleteEmployeeAttendanceMaster(employeeAttendanceId);
            return Ok(res);
        }

        [HttpPost("GetEmployeeAttendanceReport")]
        public async Task<IActionResult> GetEmployeeAttendanceReport(EmployeeAttendanceReportRequestDTO request)
        {
            var res = await _employeeAttendanceService.GetEmployeeAttendanceReport(request);
            return Ok(res);
        }

        [HttpPost("ExportEmployeeAttendanceReportToExcel")]
        public async Task<IActionResult> ExportEmployeeAttendanceReportToExcel(EmployeeAttendanceReportRequestDTO request)
        {

            var response = await _employeeAttendanceService.ExportEmployeeAttendanceReportToExcel(request);

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
