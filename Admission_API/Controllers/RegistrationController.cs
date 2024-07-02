using Admission_API.DTOs.Requests;
using Admission_API.Models;
using Admission_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Admission_API.Controllers
{
    [Route("iGuru/Registration/Registration")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost("AddRegistration")]
        public async Task<IActionResult> AddRegistration(Registration request)
        {
            var result = await _registrationService.AddRegistration(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetAllRegistration")]
        public async Task<IActionResult> GetAllRegistrations(GetAllRequest request)
        {
            var result = await _registrationService.GetAllRegistrations(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("SendRegistrationMessage/{RegistrationID}")]
        public async Task<IActionResult> SendRegistrationMessage(SendRegistrationMessageRequest request)
        {
            var result = await _registrationService.SendRegistrationMessage(request);
            return StatusCode(result.StatusCode, result);
        }
    }

    [Route("iGuru/Registration/SMSReport")]
    [ApiController]
    public class RegistrationSMSReportController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationSMSReportController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpGet("GetRegistrationSMSReport")]
        public async Task<IActionResult> GetRegistrationSMSReport()
        {
            var result = await _registrationService.GetRegistrationSMSReport();
            return StatusCode(result.StatusCode, result);
        }
    }
}
