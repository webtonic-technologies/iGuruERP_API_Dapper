using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Attendance_API.Services.Interfaces;
using Attendance_API.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        public async Task<IActionResult> GetEmployeeAttendanceStatusMasterList(int instituteId)
        {
            var response = await _employeeAttendanceStatusMasterService.GetEmployeeAttendanceStatusMasterList(instituteId);
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
        public async Task<IActionResult> SaveEmployeeAttendanceStatusMaster(List<EmployeeAttendanceStatusMasterDTO> request)
        {
            var response = await _employeeAttendanceStatusMasterService.SaveEmployeeAttendanceStatusMaster(request);
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
