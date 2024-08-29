using Microsoft.AspNetCore.Mvc;
using Student_API.Services.Interfaces;
using Student_API.DTOs.ServiceResponse;
using Student_API.DTOs.RequestDTO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Student_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class IMEIRegistrationController : ControllerBase
    {
        private readonly IIMEIRegistrationService _imeiRegistrationService;

        public IMEIRegistrationController(IIMEIRegistrationService imeiRegistrationService)
        {
            _imeiRegistrationService = imeiRegistrationService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddIMEIRegistration([FromBody] IMEIRegistrationModel imeiRegistrationDto)
        {
            var response = await _imeiRegistrationService.AddIMEIRegistration(imeiRegistrationDto);
            if (response.Success)
            {
                return CreatedAtAction(nameof(GetAllIMEIRegistrations), new { id = response.Data }, response.Message);
            }
            return BadRequest(response.Message);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllIMEIRegistrations()
        {
            var response = await _imeiRegistrationService.GetAllIMEIRegistrations();
            if (response.Success)
            {
                return Ok(response.Data);
            }
            return NotFound(response.Message);
        }
    }
}
