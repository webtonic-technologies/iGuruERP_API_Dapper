using Microsoft.AspNetCore.Mvc;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Controllers
{
    [Route("iGuru/Modules/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly IModuleService _moduleService;

        public ModulesController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        [HttpPost("GetAllModules")]
        public async Task<IActionResult> GetAllModules([FromBody] GetAllModulesRequest request)
        {
            var response = await _moduleService.GetAllModules(request);
            return StatusCode(response.StatusCode, response);
        }

        // Other endpoints for Modules
    }
}
