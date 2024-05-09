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
    public class StudentAttendanceStatusController : ControllerBase
    {
        private readonly IStudentAttendanceStatusService _studentAttendanceStatusService;

        public StudentAttendanceStatusController(IStudentAttendanceStatusService studentAttendanceStatusService)
        {
            _studentAttendanceStatusService = studentAttendanceStatusService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentAttendanceStatusList()
        {
            try
            {
                var data = await _studentAttendanceStatusService.GetStudentAttendanceStatusList();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentAttendanceStatusById(int id)
        {
            try
            {
                var data = await _studentAttendanceStatusService.GetStudentAttendanceStatusById(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddStudentAttendanceStatus([FromBody] StudentAttendanceStatusDTO request)
        {
            try
            {
                var data = await _studentAttendanceStatusService.AddStudentAttendanceStatus(request);
                if (data != null)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest("Bad Request");
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStudentAttendanceStatus([FromBody] StudentAttendanceStatusDTO request)
        {
            try
            {
                var data = await _studentAttendanceStatusService.UpdateStudentAttendanceStatus(request);
                if (data != null)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest("Bad Request");
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentAttendanceStatus(int id)
        {
            try
            {
                var data = await _studentAttendanceStatusService.DeleteStudentAttendanceStatus(id);
                if (data != null)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest("Bad Request");
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
    }
}
