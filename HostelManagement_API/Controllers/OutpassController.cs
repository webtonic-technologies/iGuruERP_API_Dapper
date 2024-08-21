using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/Hostel/[controller]")]
    [ApiController]
    public class OutpassController : ControllerBase
    {
        private readonly IOutpassService _outpassService;
        private readonly ILogger<OutpassController> _logger;

        public OutpassController(IOutpassService outpassService, ILogger<OutpassController> logger)
        {
            _outpassService = outpassService;
            _logger = logger;
        }

        [HttpPost("AddUpdateOutpass")]
        public async Task<IActionResult> AddUpdateOutpass([FromBody] AddUpdateOutpassRequest request)
        {
            _logger.LogInformation("AddUpdateOutpass Request Received: {@Request}", request);
            var response = await _outpassService.AddUpdateOutpass(request);
            _logger.LogInformation("AddUpdateOutpass Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllOutpass")]
        public async Task<IActionResult> GetAllOutpass([FromBody] GetAllOutpassRequest request)
        {
            _logger.LogInformation("GetAllOutpass Request Received: {@Request}", request);
            var response = await _outpassService.GetAllOutpass(request);
            _logger.LogInformation("GetAllOutpass Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetOutpass/{outpassId}")]
        public async Task<IActionResult> GetOutpassById(int outpassId)
        {
            _logger.LogInformation("GetOutpassById Request Received for OutpassID: {OutpassID}", outpassId);
            var response = await _outpassService.GetOutpassById(outpassId);
            _logger.LogInformation("GetOutpassById Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Delete/{outpassId}")]
        public async Task<IActionResult> DeleteOutpass(int outpassId)
        {
            _logger.LogInformation("DeleteOutpass Request Received for OutpassID: {OutpassID}", outpassId);
            var response = await _outpassService.DeleteOutpass(outpassId);
            _logger.LogInformation("DeleteOutpass Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }
    }
}
