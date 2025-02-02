using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse; 
using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/Hostel/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        [HttpPost("AddUpdateRoom")]
        public async Task<IActionResult> AddUpdateRoom([FromBody] AddUpdateRoomRequest request)
        {
            _logger.LogInformation("AddUpdateRoom Request Received: {@Request}", request);
            var response = await _roomService.AddUpdateRoom(request);
            _logger.LogInformation("AddUpdateRoom Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllRooms")]
        public async Task<IActionResult> GetAllRooms([FromBody] GetAllRoomsRequest request)
        {
            _logger.LogInformation("GetAllRooms Request Received: {@Request}", request);
            var response = await _roomService.GetAllRooms(request);
            _logger.LogInformation("GetAllRooms Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetRoom/{roomId}")]
        public async Task<IActionResult> GetRoomById(int roomId)
        {
            _logger.LogInformation("GetRoomById Request Received for RoomID: {RoomID}", roomId);
            var response = await _roomService.GetRoomById(roomId);
            _logger.LogInformation("GetRoomById Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Delete/{roomId}")]
        public async Task<IActionResult> DeleteRoom(int roomId)
        {
            _logger.LogInformation("DeleteRoom Request Received for RoomID: {RoomID}", roomId);
            var response = await _roomService.DeleteRoom(roomId);
            _logger.LogInformation("DeleteRoom Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetFloorsDDL")]
        public async Task<IActionResult> GetFloorsDDL([FromBody] GetFloorsDDLRequest request)
        {
            _logger.LogInformation("GetFloorsDDL Request Received: {@Request}", request);

            var floors = await _roomService.GetFloorsDDL(request.InstituteID);

            _logger.LogInformation("GetFloorsDDL Response: {@Response}", floors);

            return Ok(new ServiceResponse<IEnumerable<GetFloorsDDLResponse>>(true, "Floors retrieved successfully", floors, 200));
        }

    }
}
