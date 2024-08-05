using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/Hostel/[controller]")]
    [ApiController]
    public class HostelController : ControllerBase
    {
        private readonly IHostelService _hostelService;
        private readonly ILogger<HostelController> _logger;

        public HostelController(IHostelService hostelService, ILogger<HostelController> logger)
        {
            _hostelService = hostelService;
            _logger = logger;
        }

        [HttpPost("AddUpdateHostel")]
        public async Task<IActionResult> AddUpdateHostel([FromBody] AddUpdateHostelRequest request)
        {
            _logger.LogInformation("AddUpdateHostel Request Received: {@Request}", request);
            var response = await _hostelService.AddUpdateHostel(request);
            _logger.LogInformation("AddUpdateHostel Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllHostel")]
        public async Task<IActionResult> GetAllHostels([FromBody] GetAllHostelsRequest request)
        {
            _logger.LogInformation("GetAllHostels Request Received: {@Request}", request);
            var response = await _hostelService.GetAllHostels(request);
            _logger.LogInformation("GetAllHostels Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetHostel/{hostelId}")]
        public async Task<IActionResult> GetHostelById(int hostelId)
        {
            _logger.LogInformation("GetHostelById Request Received for HostelID: {HostelID}", hostelId);
            var response = await _hostelService.GetHostelById(hostelId);
            _logger.LogInformation("GetHostelById Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Delete/{hostelId}")]
        public async Task<IActionResult> DeleteHostel(int hostelId)
        {
            _logger.LogInformation("DeleteHostel Request Received for HostelID: {HostelID}", hostelId);
            var response = await _hostelService.DeleteHostel(hostelId);
            _logger.LogInformation("DeleteHostel Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }
    }
}
