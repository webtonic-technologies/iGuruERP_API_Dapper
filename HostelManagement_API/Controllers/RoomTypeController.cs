using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeService _roomTypeService;
        private readonly ILogger<RoomTypeController> _logger;

        public RoomTypeController(IRoomTypeService roomTypeService, ILogger<RoomTypeController> logger)
        {
            _roomTypeService = roomTypeService;
            _logger = logger;
        }

        [HttpPost("AddUpdateRoomTypes")]
        public async Task<IActionResult> AddUpdateRoomType([FromBody] AddUpdateRoomTypeRequest request)
        {
            _logger.LogInformation("AddUpdateRoomType Request Received: {@Request}", request);
            var response = await _roomTypeService.AddUpdateRoomType(request);
            _logger.LogInformation("AddUpdateRoomType Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllRoomTypes")]
        public async Task<IActionResult> GetAllRoomTypes([FromBody] GetAllRoomTypesRequest request)
        {
            _logger.LogInformation("GetAllRoomTypes Request Received: {@Request}", request);
            var response = await _roomTypeService.GetAllRoomTypes(request);
            _logger.LogInformation("GetAllRoomTypes Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetRoomType/{roomTypeId}")]
        public async Task<IActionResult> GetRoomType(int roomTypeId)
        {
            _logger.LogInformation("GetRoomType Request Received for RoomTypeID: {RoomTypeID}", roomTypeId);
            var response = await _roomTypeService.GetRoomTypeById(roomTypeId);
            _logger.LogInformation("GetRoomType Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Delete/{roomTypeId}")]
        public async Task<IActionResult> DeleteRoomType(int roomTypeId)
        {
            _logger.LogInformation("DeleteRoomType Request Received for RoomTypeID: {RoomTypeID}", roomTypeId);
            var response = await _roomTypeService.DeleteRoomType(roomTypeId);
            _logger.LogInformation("DeleteRoomType Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetRoomTypesDDL")]
        public async Task<IActionResult> GetRoomTypesDDL([FromBody] GetRoomTypesDDLRequest request)
        {
            _logger.LogInformation("GetRoomTypesDDL Request Received: {@Request}", request);

            var roomTypes = await _roomTypeService.GetRoomTypesDDL(request.InstituteID);

            _logger.LogInformation("GetRoomTypesDDL Response: {@Response}", roomTypes);

            return Ok(new ServiceResponse<IEnumerable<GetRoomTypesDDLResponse>>(true, "Room types retrieved successfully", roomTypes, 200));
        }

    }
}
