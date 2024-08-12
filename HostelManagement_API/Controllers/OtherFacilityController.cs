using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/Configuration/RoomTypes/[controller]")]
    [ApiController]
    public class OtherFacilityController : ControllerBase
    {
        private readonly IOtherFacilityService _otherFacilityService;
        private readonly ILogger<OtherFacilityController> _logger;

        public OtherFacilityController(IOtherFacilityService otherFacilityService, ILogger<OtherFacilityController> logger)
        {
            _otherFacilityService = otherFacilityService;
            _logger = logger;
        }

        [HttpPost("GetAllOtherFacilities")]
        public async Task<IActionResult> GetAllOtherFacilities()
        {
            _logger.LogInformation("GetAllOtherFacilities Request Received");
            var facilities = await _otherFacilityService.GetAllOtherFacilities();
            _logger.LogInformation("GetAllOtherFacilities Response: {@Response}", facilities);
            return Ok(new { Success = true, Message = "Other facilities retrieved successfully", Data = facilities });
        }
    }
}
