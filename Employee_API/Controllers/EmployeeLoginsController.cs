using Employee_API.DTOs;
using Employee_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Employee_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class EmployeeLoginsController : ControllerBase
    {
        private readonly IEmployeeLoginsServices _employeeLoginsServices;
        public EmployeeLoginsController(IEmployeeLoginsServices employeeLoginsServices)
        {
            _employeeLoginsServices = employeeLoginsServices;
        }
        
        [HttpPost("GetAllEmployeeLoginCred")]
        public async Task<IActionResult> GetAllEmployeeLoginCred(EmployeeLoginRequest request)
        {
            try
            {
                var data = await _employeeLoginsServices.GetAllEmployeeLoginCred(request);
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
        [HttpPost("GetAllEmployeeNonAppUsers")]
        public async Task<IActionResult> GetAllEmployeeNonAppUsers(EmployeeLoginRequest request)
        {
            try
            {
                var data = await _employeeLoginsServices.GetAllEmployeeNonAppUsers(request);
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
        [HttpPost("GetAllEmployeeActivity")]
        public async Task<IActionResult> GetAllEmployeeActivity(EmployeeLoginRequest request)
        {
            try
            {
                var data = await _employeeLoginsServices.GetAllEmployeeActivity(request);
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
        [HttpPost("DownloadExcelSheet")]
        public async Task<IActionResult> DownloadExcelSheet(DownloadExcelRequest request)
        {
            var response = await _employeeLoginsServices.DownloadExcelSheet(request);
            if (response.Success)
            {
                return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeCredentials.xlsx");
            }
            return StatusCode(response.StatusCode, response.Message);
        }
        [HttpPost("DownloadExcelSheetNonAppUsers")]
        public async Task<IActionResult> DownloadExcelSheetNonAppUsers(DownloadExcelRequest request)
        {
            var response = await _employeeLoginsServices.DownloadExcelSheetNonAppUsers(request);
            if (response.Success)
            {
                return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeNonAppUsers.xlsx");
            }
            return StatusCode(response.StatusCode, response.Message);
        }
        [HttpPost("DownloadExcelSheetEmployeeActivity")]
        public async Task<IActionResult> DownloadExcelSheetEmployeeActivity(DownloadExcelRequest request)
        {
            var response = await _employeeLoginsServices.DownloadExcelSheetEmployeeActivity(request);
            if (response.Success)
            {
                return File(response.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeActivity.xlsx");
            }
            return StatusCode(response.StatusCode, response.Message);
        }
        [HttpPost("UserLogin")]
        public async Task<IActionResult> UserLogin(string username)
        {
            try
            {
                var data = await _employeeLoginsServices.UserLogin(username);
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
        [HttpPost("UserLoginPasswordScreen")]
        public async Task<IActionResult> UserLoginPasswordScreen(UserLoginRequest request)
        {
            try
            {
                var data = await _employeeLoginsServices.UserLoginPasswordScreen(request);
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
        [HttpPost("UserLogout")]
        public async Task<IActionResult> UserLogout(string username)
        {
            try
            {
                var data = await _employeeLoginsServices.UserLogout(username);
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
