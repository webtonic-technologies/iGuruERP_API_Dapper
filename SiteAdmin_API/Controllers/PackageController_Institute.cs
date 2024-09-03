using Microsoft.AspNetCore.Mvc;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Controllers
{
    [Route("iGuru/CreatePackage/[controller]")]
    [ApiController]
    public class PackageController_Institute : ControllerBase
    {
        private readonly IPackageService_Institute _packageService;

        public PackageController_Institute(IPackageService_Institute packageService)
        {
            _packageService = packageService;
        }

        [HttpGet("GetAllPackage_Institute")]
        public async Task<IActionResult> GetAllPackagesForInstitute()
        {
            var response = await _packageService.GetAllPackagesForInstitute();
            return StatusCode(response.StatusCode, response);
        }
    }
}
