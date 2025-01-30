using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        private readonly IBuildingService _buildingService;
        private readonly ILogger<BuildingController> _logger;

        public BuildingController(IBuildingService buildingService, ILogger<BuildingController> logger)
        {
            _buildingService = buildingService;
            _logger = logger;
        }

        [HttpPost("AddUpdateBuildings")]
        public async Task<IActionResult> AddUpdateBuildings([FromBody] AddUpdateBuildingsRequest request)
        {
            _logger.LogInformation("AddUpdateBuildings Request Received: {@Request}", request);
            var result = await _buildingService.AddUpdateBuildings(request);
            _logger.LogInformation("AddUpdateBuildings Response: {@Response}", result);
            return Ok(result);
        }

        [HttpPost("GetAllBuildings")]
        public async Task<IActionResult> GetAllBuildings([FromBody] GetAllBuildingsRequest request)
        {
            _logger.LogInformation("GetAllBuildings Request Received: {@Request}", request);
            var response = await _buildingService.GetAllBuildings(request);
            _logger.LogInformation("GetAllBuildings Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllBuildings_Fetch")]
        public async Task<IActionResult> GetAllBuildingsFetch([FromBody] GetAllBuildingsFetchRequest request)
        {
            _logger.LogInformation("GetAllBuildingsFetch Request Received with InstituteID: {@InstituteID}", request.InstituteID);

            // Pass InstituteID from the request to the service
            IEnumerable<BuildingFetchResponse> buildings = await _buildingService.GetAllBuildingsFetch(request.InstituteID);

            _logger.LogInformation("GetAllBuildingsFetch Response: {@Response}", buildings);
            return Ok(new { Success = true, Message = "Buildings retrieved successfully", Data = buildings });
        }


        [HttpGet("GetBuilding/{buildingId}")]
        public async Task<IActionResult> GetBuilding(int buildingId)
        {
            _logger.LogInformation("GetBuilding Request Received for BuildingID: {BuildingID}", buildingId);
            var response = await _buildingService.GetBuildingById(buildingId);
            _logger.LogInformation("GetBuilding Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Delete/{buildingId}")]
        public async Task<IActionResult> DeleteBuilding(int buildingId)
        {
            _logger.LogInformation("DeleteBuilding Request Received for BuildingID: {BuildingID}", buildingId);
            var response = await _buildingService.DeleteBuilding(buildingId);
            _logger.LogInformation("DeleteBuilding Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }
    }
}
