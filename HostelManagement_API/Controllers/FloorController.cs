using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class FloorController : ControllerBase
    {
        private readonly IFloorService _floorService;
        private readonly ILogger<FloorController> _logger;

        public FloorController(IFloorService floorService, ILogger<FloorController> logger)
        {
            _floorService = floorService;
            _logger = logger;
        }

        [HttpPost("AddUpdateFloors")]
        public async Task<IActionResult> AddUpdateFloor([FromBody] AddUpdateFloorRequest request)
        {
            _logger.LogInformation("AddUpdateFloor Request Received: {@Request}", request);
            var response = await _floorService.AddUpdateFloor(request);
            _logger.LogInformation("AddUpdateFloor Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllFloors")]
        public async Task<IActionResult> GetAllFloors([FromBody] GetAllFloorsRequest request)
        {
            _logger.LogInformation("GetAllFloors Request Received: {@Request}", request);
            var response = await _floorService.GetAllFloors(request);
            _logger.LogInformation("GetAllFloors Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetFloor/{floorId}")]
        public async Task<IActionResult> GetFloor(int floorId)
        {
            _logger.LogInformation("GetFloor Request Received for FloorID: {FloorID}", floorId);
            var response = await _floorService.GetFloorById(floorId);
            _logger.LogInformation("GetFloor Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Delete/{floorId}")]
        public async Task<IActionResult> DeleteFloor(int floorId)
        {
            _logger.LogInformation("DeleteFloor Request Received for FloorID: {FloorID}", floorId);
            var response = await _floorService.DeleteFloor(floorId);
            _logger.LogInformation("DeleteFloor Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }
    }
}
