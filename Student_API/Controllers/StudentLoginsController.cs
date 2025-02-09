using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
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
        public async Task<IActionResult> DownloadExcelSheet(int InstituteId, string format)
        {
            var response = await _studentLoginsServices.DownloadExcelSheet(InstituteId, format);
            if (response.Success)
            {
                if (format.ToLower() == "excel")
                {
                    return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentCredentials.xlsx");
                }

                else if (format.ToLower() == "csv")
                {
                    return File(response.Data, "text/csv", "StudentCredentials.csv");
                }
            }
            return StatusCode(response.StatusCode, response.Message);
        }
        [HttpGet("DownloadExcelSheetNonAppUsers/{InstituteId}")]
        public async Task<IActionResult> DownloadExcelSheetNonAppUsers(int InstituteId, string format)
        {
            var response = await _studentLoginsServices.DownloadExcelSheetNonAppUsers(InstituteId, format);
            if (response.Success)
            {
                if (format.ToLower() == "excel")
                {
                    return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentNonAppUsers.xlsx");
                }
                else if (format.ToLower() == "csv")
                {
                    return File(response.Data, "text/csv", "StudentNonAppUsers.csv");
                }
            }
            return StatusCode(response.StatusCode, response.Message);
        }
        [HttpGet("DownloadExcelSheetStudentActivity/{InstituteId}")]
        public async Task<IActionResult> DownloadExcelSheetStudentActivity(int InstituteId, string format)
        {
            var response = await _studentLoginsServices.DownloadExcelSheetStudentActivity(InstituteId, format);
            if (response.Success)
            {
                if (format.ToLower() == "excel")
                {
                    return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentActivity.xlsx");
                }
                else if (format.ToLower() == "csv")
                {
                    return File(response.Data, "text/csv", "StudentActivity.csv");
                }
            }
            return StatusCode(response.StatusCode, response.Message);
        }
    }
}