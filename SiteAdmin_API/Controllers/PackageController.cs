using Microsoft.AspNetCore.Mvc;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Controllers
{
    [Route("iGuru/CreatePackage/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpPost("AddUpdatePackage")]
        public async Task<IActionResult> AddUpdatePackage([FromBody] AddUpdatePackageRequest request)
        {
            var response = await _packageService.AddUpdatePackage(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllPackage")]
        public async Task<IActionResult> GetAllPackages()
        {
            var response = await _packageService.GetAllPackages();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("Status")]
        public async Task<IActionResult> UpdatePackageStatus([FromForm] int packageId)
        {
            var response = await _packageService.UpdatePackageStatus(packageId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
