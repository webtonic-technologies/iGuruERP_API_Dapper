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
        public async Task<IActionResult> GetStudentAttendanceStatusList(int InstituteId)
        {
            try
            {
                var data = await _studentAttendanceStatusService.GetStudentAttendanceStatusList(InstituteId);
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

        [HttpPost("AddEditStudentAttendanceStatus")]
        public async Task<IActionResult> AddEditStudentAttendanceStatus([FromBody] List<StudentAttendanceStatusDTO> request)
        {
            try
            {
                var data = await _studentAttendanceStatusService.SaveStudentAttendanceStatus(request);
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

        //[HttpPut]
        //public async Task<IActionResult> UpdateStudentAttendanceStatus([FromBody] StudentAttendanceStatusDTO request)
        //{
        //    try
        //    {
        //        var data = await _studentAttendanceStatusService.SaveStudentAttendanceStatus(request);
        //        if (data != null)
        //        {
        //            return Ok(data);
        //        }
        //        else
        //        {
        //            return BadRequest("Bad Request");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return this.BadRequest(e.Message);
        //    }
        //}

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
