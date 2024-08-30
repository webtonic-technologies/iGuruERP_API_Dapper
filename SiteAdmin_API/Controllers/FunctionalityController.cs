using Microsoft.AspNetCore.Mvc;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Controllers
{
    [Route("iGuru/SubModules/[controller]")]
    [ApiController]
    public class FunctionalityController : ControllerBase
    {
        private readonly IFunctionalityService _functionalityService;

        public FunctionalityController(IFunctionalityService functionalityService)
        {
            _functionalityService = functionalityService;
        }

        [HttpPost("UpdateFunctionality")]
        public async Task<IActionResult> UpdateFunctionality([FromBody] UpdateFunctionalityRequest request)
        {
            var response = await _functionalityService.UpdateFunctionality(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("Status")]
        public async Task<IActionResult> UpdateFunctionalityStatus([FromForm] int functionalityId)
        {
            var response = await _functionalityService.UpdateFunctionalityStatus(functionalityId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
