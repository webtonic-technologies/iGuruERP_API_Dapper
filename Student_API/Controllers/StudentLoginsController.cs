﻿using Microsoft.AspNetCore.Mvc;
using Student_API.DTOs.RequestDTO;
using Student_API.Services.Interfaces;

namespace Student_API.Controllers
{
    [Route("iGuru/StudentLogins/[controller]")]
    [ApiController]
    public class StudentLoginsController : ControllerBase
    {
        private readonly IStudentLoginsServices _studentLoginsServices; 
        public StudentLoginsController(IStudentLoginsServices studentLoginsServices)
        {
            _studentLoginsServices = studentLoginsServices;
        }
        [HttpPost("GetAllStudentLoginCred")]
        public async Task<IActionResult> GetAllStudentLoginCred(StudentLoginRequest request)
        {
            try
            {
                var data = await _studentLoginsServices.GetAllStudentLoginCred(request);
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
        [HttpPost("GetAllStudentNonAppUsers")]
        public async Task<IActionResult> GetAllStudentNonAppUsers(StudentLoginRequest request)
        {
            try
            {
                var data = await _studentLoginsServices.GetAllStudentNonAppUsers(request);
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
        [HttpPost("GetAllStudentActivity")]
        public async Task<IActionResult> GetAllStudentActivity(StudentLoginRequest request)
        {
            try
            {
                var data = await _studentLoginsServices.GetAllStudentActivity(request);
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
        [HttpGet("DownloadExcelSheet/{InstituteId}")]
        public async Task<IActionResult> DownloadExcelSheet(int InstituteId)
        {
            var response = await _studentLoginsServices.DownloadExcelSheet(InstituteId);
            if (response.Success)
            {
                return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentCredentials.xlsx");
            }
            return StatusCode(response.StatusCode, response.Message);
        }
        [HttpGet("DownloadExcelSheetNonAppUsers/{InstituteId}")]
        public async Task<IActionResult> DownloadExcelSheetNonAppUsers(int InstituteId)
        {
            var response = await _studentLoginsServices.DownloadExcelSheetNonAppUsers(InstituteId);
            if (response.Success)
            {
                return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentNonAppUsers.xlsx");
            }
            return StatusCode(response.StatusCode, response.Message);
        }
        [HttpGet("DownloadExcelSheetStudentActivity/{InstituteId}")]
        public async Task<IActionResult> DownloadExcelSheetStudentActivity(int InstituteId)
        {
            var response = await _studentLoginsServices.DownloadExcelSheetStudentActivity(InstituteId);
            if (response.Success)
            {
                return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentActivity.xlsx");
            }
            return StatusCode(response.StatusCode, response.Message);
        }
    }
}