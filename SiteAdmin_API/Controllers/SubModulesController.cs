using Microsoft.AspNetCore.Mvc;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Controllers
{
    [Route("iGuru/SubModules/[controller]")]
    [ApiController]
    public class SubModulesController : ControllerBase
    {
        private readonly ISubModuleService _subModuleService;

        public SubModulesController(ISubModuleService subModuleService)
        {
            _subModuleService = subModuleService;
        }

        [HttpPost("GetAllSubModules")]
        public async Task<IActionResult> GetAllSubModules([FromBody] GetAllSubModulesRequest request)
        {
            var response = await _subModuleService.GetAllSubModules(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("GetAllFunctionality")]
        public async Task<IActionResult> GetAllFunctionality([FromBody] GetAllFunctionalityRequest request)
        {
            var response = await _subModuleService.GetAllFunctionality(request);
            return StatusCode(response.StatusCode, response);
        }

    }
}
