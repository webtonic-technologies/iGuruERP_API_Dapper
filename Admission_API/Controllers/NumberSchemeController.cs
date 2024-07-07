using Admission_API.DTOs.Requests;
using Admission_API.Models;
using Admission_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Admission_API.Controllers
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

        [HttpPost("AddUpdateScheme")]
        public async Task<IActionResult> AddUpdateNumberScheme(NumberScheme request)
        {
            var result = await _numberSchemeService.AddUpdateNumberScheme(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetAllScheme")]
        public async Task<IActionResult> GetAllNumberSchemes(GetAllRequest request)
        {
            var result = await _numberSchemeService.GetAllNumberSchemes(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetScheme/{SchemeID}")]
        public async Task<IActionResult> GetNumberSchemeById(int SchemeID)
        {
            var result = await _numberSchemeService.GetNumberSchemeById(SchemeID);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Delete/{SchemeID}")]
        public async Task<IActionResult> DeleteNumberScheme(int SchemeID)
        {
            var result = await _numberSchemeService.DeleteNumberScheme(SchemeID);
            return StatusCode(result.StatusCode, result);
        }
    }
}
