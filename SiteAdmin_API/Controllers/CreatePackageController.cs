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

        [HttpPost("UpdatePackage")]
        public async Task<IActionResult> UpdatePackage([FromBody] UpdatePackageRequest request)
        {
            var response = await _createPackageService.UpdatePackage(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("Status")]
        public async Task<IActionResult> UpdatePackageStatus([FromForm] int packageId)
        {
            var response = await _createPackageService.UpdatePackageStatus(packageId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
