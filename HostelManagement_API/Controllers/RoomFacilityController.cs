using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/Configuration/RoomTypes/[controller]")]
    [ApiController]
    public class RoomFacilityController : ControllerBase
    {
        private readonly IRoomFacilityService _roomFacilityService;
        private readonly ILogger<RoomFacilityController> _logger;

        public RoomFacilityController(IRoomFacilityService roomFacilityService, ILogger<RoomFacilityController> logger)
        {
            _roomFacilityService = roomFacilityService;
            _logger = logger;
        }

        [HttpPost("GetAllRoomFacilities")]
        public async Task<IActionResult> GetAllRoomFacilities()
        {
            _logger.LogInformation("GetAllRoomFacilities Request Received");
            var facilities = await _roomFacilityService.GetAllRoomFacilities();
            _logger.LogInformation("GetAllRoomFacilities Response: {@Response}", facilities);
            return Ok(new { Success = true, Message = "Room facilities retrieved successfully", Data = facilities });
        }
    }
}
