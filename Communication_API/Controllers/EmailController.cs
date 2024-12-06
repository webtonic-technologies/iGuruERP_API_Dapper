using Communication_API.DTOs.Requests.Email;
using Communication_API.Services.Interfaces.Email;
using Microsoft.AspNetCore.Mvc;

namespace Communication_API.Controllers
{
    [Route("iGuru/Email/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("ConfigureEmail")]
        public async Task<IActionResult> ConfigureEmail([FromBody] ConfigureEmailRequest request)
        {
            var response = await _emailService.ConfigureEmail(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("SendNewEmail")]
        public async Task<IActionResult> SendNewEmail([FromBody] SendNewEmailRequest request)
        {
            var response = await _emailService.SendNewEmail(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetEmailReports")]
        public async Task<IActionResult> GetEmailReports([FromBody] GetEmailReportsRequest request)
        {
            var response = await _emailService.GetEmailReports(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("SendEmailStudent")]
        public async Task<IActionResult> SendEmailStudent(SendEmailStudentRequest request)
        {
            // Call the service layer to send email to students
            var response = await _emailService.SendEmailToStudents(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }

        [HttpPost("SendEmailEmployee")]
        public async Task<IActionResult> SendEmailEmployee(SendEmailEmployeeRequest request)
        {
            // Call the service layer to send email to employees
            var response = await _emailService.SendEmailToEmployees(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }

        [HttpPost("UpdateEmailStudentStatus")]
        public async Task<IActionResult> UpdateEmailStudentStatus(UpdateEmailStudentStatusRequest request)
        {
            // Call the service layer to update the email status for the student
            var response = await _emailService.UpdateEmailStudentStatus(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }

        [HttpPost("UpdateEmailEmployeeStatus")]
        public async Task<IActionResult> UpdateEmailEmployeeStatus(UpdateEmailEmployeeStatusRequest request)
        {
            // Call the service layer to update the email status for the employee
            var response = await _emailService.UpdateEmailEmployeeStatus(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }
    }
}
