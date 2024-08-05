using Microsoft.AspNetCore.Mvc;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Controllers
{
    [Route("iGuru/CreatePackage/[controller]")]
    [ApiController]
    public class CreatePackageController : ControllerBase
    {
        private readonly ICreatePackageService _createPackageService;

        public CreatePackageController(ICreatePackageService createPackageService)
        {
            _createPackageService = createPackageService;
        }

        [HttpPost("CreatePackage")]
        public async Task<IActionResult> CreatePackage([FromBody] CreatePackageRequest request)
        {
            var response = await _createPackageService.CreatePackage(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
