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

        [HttpPost("GetAllInstituteOnboard")]
        public async Task<IActionResult> GetAllInstituteOnboard([FromBody] GetAllInstituteOnboardRequest request)
        {
            var response = await _instituteOnboardService.GetAllInstituteOnboard(request.PageNumber, request.PageSize);
            return StatusCode(response.StatusCode, response);
        }


        [HttpGet("GetInstituteOnboard/{instituteOnboardId}")]
        public async Task<IActionResult> GetInstituteOnboardById(int instituteOnboardId)
        {
            var response = await _instituteOnboardService.GetInstituteOnboardById(instituteOnboardId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("UpgradePackage")]
        public async Task<IActionResult> UpgradePackage([FromBody] UpgradePackageRequest request)
        {
            var response = await _instituteOnboardService.UpgradePackage(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetPackageDDL")]
        public async Task<IActionResult> GetPackageDDL()
        {
            var response = await _instituteOnboardService.GetPackageDDL();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllInstituteInfo")]
        public async Task<IActionResult> GetAllInstituteInfo([FromBody] GetAllInstituteInfoRequest request)
        {
            var response = await _instituteOnboardService.GetAllInstituteInfo(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("AddAdmissionURL")]
        public async Task<IActionResult> AddAdmissionURL([FromBody] AddAdmissionURLRequest request)
        {
            var response = await _instituteOnboardService.AddAdmissionURL(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("ActivityLogs")]
        public async Task<IActionResult> GetActivityLogs([FromBody] ActivityLogsRequest request)
        {
            var response = await _instituteOnboardService.GetActivityLogs(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
