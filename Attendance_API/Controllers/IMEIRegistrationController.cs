using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;
using System.Collections.Generic;
using Attendance_API.Services.Interfaces;
using Attendance_API.DTOs;

namespace Attendance_API.Controllers
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
