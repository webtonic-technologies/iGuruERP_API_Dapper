using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Attendance_API.Services.Interfaces;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using System.Threading.Tasks;

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
    }
}
