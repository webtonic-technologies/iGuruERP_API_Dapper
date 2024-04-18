using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Attendance_API.Services.Interfaces;
using Attendance_API.DTOs;
using System.Threading.Tasks;

namespace Attendance_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class EmployeeAttendanceStatusController : ControllerBase
    {
        private readonly IEmployeeAttendanceStatusMasterService _employeeAttendanceStatusMasterService;

        public EmployeeAttendanceStatusController(IEmployeeAttendanceStatusMasterService employeeAttendanceStatusMasterService)
        {
            _employeeAttendanceStatusMasterService = employeeAttendanceStatusMasterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeAttendanceStatusMasterList()
        {
            var response = await _employeeAttendanceStatusMasterService.GetEmployeeAttendanceStatusMasterList();
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeAttendanceStatusMasterById(int id)
        {
            var response = await _employeeAttendanceStatusMasterService.GetEmployeeAttendanceStatusMasterById(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployeeAttendanceStatusMaster(EmployeeAttendanceStatusMasterDTO request)
        {
            var response = await _employeeAttendanceStatusMasterService.AddEmployeeAttendanceStatusMaster(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployeeAttendanceStatusMaster(EmployeeAttendanceStatusMasterDTO request)
        {
            var response = await _employeeAttendanceStatusMasterService.UpdateEmployeeAttendanceStatusMaster(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeAttendanceStatusMaster(int id)
        {
            var response = await _employeeAttendanceStatusMasterService.DeleteEmployeeAttendanceStatusMaster(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
    }
}
