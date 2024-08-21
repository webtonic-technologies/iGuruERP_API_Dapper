using Microsoft.AspNetCore.Mvc;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Controllers
{
    [Route("iGuru/InstituteOnboard/[controller]")]
    [ApiController]
    public class InstituteOnboardController : ControllerBase
    {
        private readonly IInstituteOnboardService _instituteOnboardService;

        public InstituteOnboardController(IInstituteOnboardService instituteOnboardService)
        {
            _instituteOnboardService = instituteOnboardService;
        }

        [HttpPost("AddUpdateInstituteOnboard")]
        public async Task<IActionResult> AddUpdateInstituteOnboard([FromBody] InstituteOnboardRequest request)
        {
            var response = await _instituteOnboardService.AddUpdateInstituteOnboard(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetAllInstituteOnboard")]
        public async Task<IActionResult> GetAllInstituteOnboard()
        {
            var response = await _instituteOnboardService.GetAllInstituteOnboard();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetInstituteOnboard/{instituteOnboardId}")]
        public async Task<IActionResult> GetInstituteOnboardById(int instituteOnboardId)
        {
            var response = await _instituteOnboardService.GetInstituteOnboardById(instituteOnboardId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
