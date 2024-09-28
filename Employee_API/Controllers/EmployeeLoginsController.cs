using Employee_API.DTOs;
using Employee_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Employee_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeLoginsController : ControllerBase
    {
        private readonly IEmployeeLoginsServices _employeeLoginsServices;
        private readonly IConfiguration _config;
        public EmployeeLoginsController(IEmployeeLoginsServices employeeLoginsServices, IConfiguration configuration)
        {
            _employeeLoginsServices = employeeLoginsServices;
            _config = configuration;
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<IActionResult> UserLoginPasswordScreen(UserLoginRequest request)
        {
            try
            {
                var jwtToken = new JwtHelper(_config);
                var result = await _employeeLoginsServices.UserLoginPasswordScreen(request);
                if (result.Data != null)
                {
                    var status = true;
                    var message = "Login successful";
                    var token = jwtToken.GenerateJwtToken(result.Data.UserId, result.Data.UserType, result.Data.Username);
                    var data = result;
                    return this.Ok(new { status, message, data, token });
                }
                else
                {
                    var status = false;
                    var message = "Invalid Username or Password";
                    return this.BadRequest(new { status, message });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message)
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                };
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
        [HttpPost("UserSwitchOver")]
        [AllowAnonymous]
        public async Task<IActionResult> UserSwitchOver(UserSwitchOverRequest request)
        {
            try
            {
                var data = await _employeeLoginsServices.UserSwitchOver(request);
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
        [HttpPost]
        [Route("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassword forgotPassword)
        {
            try
            {
                var result = await _employeeLoginsServices.ForgetPassword(forgotPassword);
                var jwtToken = new JwtHelper(_config);
                if (result != null)
                {

                    var token = jwtToken.GenerateJwtToken(result.Data.UserId, result.Data.Usertype, result.Data.UserName);
                    var email = new SendEmail();
                    result.Data.Token = token;
                    var succes = email.SendEmailWithAttachmentAsync(result.Data.Email, token);
                    return succes.Result.Success
                     ? this.Ok(new { result })
                     : this.BadRequest(new { result});

                }
                else
                {
                    var status = true;
                    var message = "Email not verified ";
                    return this.NotFound(new { status, message });
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        [HttpPost]
        [Route("ResetPassword")]
        [AllowAnonymous]
        public async Task< IActionResult> ResetPassword([FromBody] ResetPassword request)
        {
            try
            {
                var data = await _employeeLoginsServices.ResetPassword(request);
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
