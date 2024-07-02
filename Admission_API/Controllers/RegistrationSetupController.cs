using Admission_API.DTOs.Requests;
using Admission_API.Models;
using Admission_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Admission_API.Controllers
{
    [Route("iGuru/Configuration/RegistrationSetup")]
    [ApiController]
    public class RegistrationSetupController : ControllerBase
    {
        private readonly IRegistrationSetupService _registrationSetupService;

        public RegistrationSetupController(IRegistrationSetupService registrationSetupService)
        {
            _registrationSetupService = registrationSetupService;
        }

        [HttpPost("AddUpdateRegistrationSetup")]
        public async Task<IActionResult> AddUpdateRegistrationSetup(RegistrationSetup request)
        {
            var result = await _registrationSetupService.AddUpdateRegistrationSetup(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetAllRegistrationSetup")]
        public async Task<IActionResult> GetAllRegistrationSetups(GetAllRequest request)
        {
            var result = await _registrationSetupService.GetAllRegistrationSetups(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Delete/{RegistrationSetupID}")]
        public async Task<IActionResult> DeleteRegistrationSetup(int RegistrationSetupID)
        {
            var result = await _registrationSetupService.DeleteRegistrationSetup(RegistrationSetupID);
            return StatusCode(result.StatusCode, result);
        }
    }
}
