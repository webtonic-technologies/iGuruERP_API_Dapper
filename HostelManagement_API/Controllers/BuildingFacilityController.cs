using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/Configuration/RoomTypes/[controller]")]
    [ApiController]
    public class BuildingFacilityController : ControllerBase
    {
        private readonly IBuildingFacilityService _buildingFacilityService;
        private readonly ILogger<BuildingFacilityController> _logger;

        public BuildingFacilityController(IBuildingFacilityService buildingFacilityService, ILogger<BuildingFacilityController> logger)
        {
            _buildingFacilityService = buildingFacilityService;
            _logger = logger;
        }

        [HttpPost("GetAllBuildingFacilities")]
        public async Task<IActionResult> GetAllBuildingFacilities()
        {
            _logger.LogInformation("GetAllBuildingFacilities Request Received");
            var facilities = await _buildingFacilityService.GetAllBuildingFacilities();
            _logger.LogInformation("GetAllBuildingFacilities Response: {@Response}", facilities);
            return Ok(new { Success = true, Message = "Building facilities retrieved successfully", Data = facilities });
        }
    }
}
