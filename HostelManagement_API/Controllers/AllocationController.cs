using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse; 
using HostelManagement_API.Models;
using HostelManagement_API.Services.Implementations;
using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/Hostel/Allocation")]
    [ApiController]
    public class AllocationController : ControllerBase
    {
        private readonly IAllocationService _allocationService;
        private readonly ILogger<AllocationController> _logger;

        public AllocationController(IAllocationService allocationService, ILogger<AllocationController> logger)
        {
            _allocationService = allocationService;
            _logger = logger;
        }

        [HttpPost("GetStudent")]
        public async Task<IActionResult> GetStudent([FromBody] GetStudentRequest request)
        {
            _logger.LogInformation("GetStudent Request Received: {@Request}", request);
            var result = await _allocationService.GetStudents(request);
            _logger.LogInformation("GetStudent Response: {@Response}", result);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("AllocateHostel")]
        public async Task<IActionResult> AllocateHostel([FromBody] AllocateHostelRequest request)
        {
            _logger.LogInformation("AllocateHostel Request Received: {@Request}", request);
            var result = await _allocationService.AllocateHostel(request);
            _logger.LogInformation("AllocateHostel Response: {@Response}", result);
            return StatusCode(result.StatusCode, result);
        } 

        [HttpPost("VacateHostel")]
        public async Task<IActionResult> VacateHostel([FromBody] VacateHostelRequest request)
        {
            _logger.LogInformation("VacateHostel Request Received: {@Request}", request);
            var result = await _allocationService.VacateHostel(request);
            _logger.LogInformation("VacateHostel Response: {@Response}", result);
            return Ok(result);
        }

        [HttpPost("GetHostelHistory")]
        public async Task<IActionResult> GetHostelHistory([FromBody] GetHostelHistoryRequest request)
        {
            var result = await _allocationService.GetHostelHistory(request);
            return Ok(result);
        }

        [HttpPost("GetHostel")]
        public async Task<IActionResult> GetHostel([FromBody] GetHostelRequest request)
        {
            //_logger.LogInformation("GetHostel Request Received: {@Request}", request);
            //var result = await _allocationService.GetHostel(request);
            //_logger.LogInformation("GetHostel Response: {@Response}", result);
            ////return Ok(result);
            //return Ok(new { Success = true, Message = "Hostels retrieved successfully", Data = result });


            _logger.LogInformation("GetHostel Request Received: {@Request}", request);
            var result = await _allocationService.GetHostel(request);

            var response = new ServiceResponse<IEnumerable<GetHostelResponse>>(
                success: true,
                message: "Hostel retrieved successfully",
                data: result,
                statusCode: 200
            );

            _logger.LogInformation("GetHostel Response: {@Response}", response);
            return Ok(response);

        }

        [HttpPost("GetHostelRooms")]
        public async Task<IActionResult> GetHostelRooms([FromBody] GetHostelRoomsRequest request)
        {
            _logger.LogInformation("GetHostelRooms Request Received: {@Request}", request);
            var result = await _allocationService.GetHostelRooms(request);

            var response = new ServiceResponse<IEnumerable<GetHostelRoomsResponse>>(
                success: true,
                message: "Hostel rooms retrieved successfully",
                data: result,
                statusCode: 200
            );

            _logger.LogInformation("GetHostelRooms Response: {@Response}", response);
            return Ok(response);
        }


        [HttpPost("GetHostelRoomBeds")]
        public async Task<IActionResult> GetHostelRoomBeds([FromBody] GetHostelRoomBedsRequest request)
        {

            _logger.LogInformation("GetHostelRoomBeds Request Received: {@Request}", request);
            var result = await _allocationService.GetHostelRoomBeds(request);

            var response = new ServiceResponse<IEnumerable<GetHostelRoomBedsResponse>>(
                success: true,
                message: "Hostel Room Beds Retrieved Successfully",
                data: result,
                statusCode: 200
            );

            _logger.LogInformation("GetHostelRoomBeds Response: {@Response}", response);
            return Ok(response);


            //var response = await _allocationService.GetHostelRoomBeds(request);

            //return Ok(new
            //{
            //    Success = true,
            //    Message = "Hostel Room Beds Retrieved Successfully",
            //    Data = response
            //});
        }
    }
}
