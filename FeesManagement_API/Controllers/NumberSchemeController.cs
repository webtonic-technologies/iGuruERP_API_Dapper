using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/Configuration/NumberScheme")]
    [ApiController]
    public class NumberSchemeController : ControllerBase
    {
        private readonly INumberSchemeService _numberSchemeService;

        public NumberSchemeController(INumberSchemeService numberSchemeService)
        {
            _numberSchemeService = numberSchemeService;
        }

        [HttpPost("AddUpdateNumberScheme")]
        public async Task<IActionResult> AddUpdateNumberScheme([FromBody] AddUpdateNumberSchemeRequest request)
        {
            var response = await _numberSchemeService.AddUpdateNumberScheme(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetAllNumberSchemes")]
        public async Task<IActionResult> GetAllNumberSchemes([FromBody] GetAllNumberSchemesRequest request)
        {
            var response = await _numberSchemeService.GetAllNumberSchemes(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetNumberScheme/{numberSchemeID}")]
        public async Task<IActionResult> GetNumberSchemeById(int numberSchemeID)
        {
            var response = await _numberSchemeService.GetNumberSchemeById(numberSchemeID);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("Delete/{numberSchemeID}")]
        public async Task<IActionResult> Delete(int numberSchemeID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _numberSchemeService.UpdateNumberSchemeStatus(numberSchemeID);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}
