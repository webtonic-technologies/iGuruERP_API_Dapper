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

        // Existing method to get all modules
        [HttpPost("GetAllModules")]
        public async Task<IActionResult> GetAllModules()
        {
            var response = await _moduleService.GetAllModules();
            return StatusCode(response.StatusCode, response);
        }

        // New method to update a module
        [HttpPost("UpdateModules")]
        public async Task<IActionResult> UpdateModule([FromBody] UpdateModuleRequest request)
        {
            var response = await _moduleService.UpdateModule(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("Status")]
        public async Task<IActionResult> UpdateModuleStatus([FromForm] int moduleId)
        {
            var response = await _moduleService.UpdateModuleStatus(moduleId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
