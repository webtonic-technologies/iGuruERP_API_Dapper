using Microsoft.AspNetCore.Mvc;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Controllers
{
    [Route("iGuru/InstituteOnboard/[controller]")]
    [ApiController]
    public class InstituteOnboardController_Credentials : ControllerBase
    {
        private readonly IInstituteOnboardService_Credentials _instituteOnboardService;

        public InstituteOnboardController_Credentials(IInstituteOnboardService_Credentials instituteOnboardService)
        {
            _instituteOnboardService = instituteOnboardService;
        }

        [HttpGet("GenerateInstituteCredentials")]
        public IActionResult GenerateInstituteCredentials([FromQuery] string instituteName)
        {
            var response = _instituteOnboardService.GenerateInstituteCredentials(instituteName);
            return StatusCode(response.StatusCode, response);
        }
    }
}
