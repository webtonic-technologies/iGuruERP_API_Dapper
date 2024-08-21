using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/Hostel/Hostel/[controller]")]
    [ApiController]
    public class HostelFetchController : ControllerBase
    {
        private readonly IHostelFetchService _hostelFetchService;
        private readonly ILogger<HostelFetchController> _logger;

        public HostelFetchController(IHostelFetchService hostelFetchService, ILogger<HostelFetchController> logger)
        {
            _hostelFetchService = hostelFetchService;
            _logger = logger;
        }

        [HttpPost("GetAllHostel_Fetch")]
        public async Task<IActionResult> GetAllHostels([FromBody] GetAllHostelFetchRequest request)
        {
            _logger.LogInformation("GetAllHostels Request Received for InstituteID: {InstituteID}", request.InstituteID);
            var hostels = await _hostelFetchService.GetAllHostels(request.InstituteID);
            _logger.LogInformation("GetAllHostels Response: {@Response}", hostels);
            return Ok(new { Success = true, Message = "Hostels retrieved successfully", Data = hostels });
        }
    }
}
