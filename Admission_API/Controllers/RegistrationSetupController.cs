using Admission_API.DTOs.Requests;
using Admission_API.Models;
using Admission_API.Services.Implementations;
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
        public async Task<IActionResult> AddUpdateRegistrationSetup([FromBody] RegistrationSetup request)
        {
            var result = await _registrationSetupService.AddUpdateRegistrationSetup(request, request.Options);
            return StatusCode(result.StatusCode, result);
        } 

        [HttpPost("GetAllRegistrationSetup")]
        public async Task<IActionResult> GetAllRegistrationSetup(GetAllRequest request)
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

        [HttpPut("FormStatus/{RegistrationSetupID}")]
        public async Task<IActionResult> FormStatus(int RegistrationSetupID)
        {
            var result = await _registrationSetupService.FormStatus(RegistrationSetupID);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("MandatoryStatus/{RegistrationSetupID}")]
        public async Task<IActionResult> MandatoryStatus(int RegistrationSetupID)
        {
            var result = await _registrationSetupService.MandatoryStatus(RegistrationSetupID);
            return StatusCode(result.StatusCode, result);
        }
    }
}
