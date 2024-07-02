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
    }
}
