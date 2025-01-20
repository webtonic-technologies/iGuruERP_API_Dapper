using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/MealManagement/[controller]")]
    [ApiController]
    public class MealManagementController : ControllerBase
    {
        private readonly IMealManagementService _mealManagementService;
        private readonly ILogger<MealManagementController> _logger;

        public MealManagementController(IMealManagementService mealManagementService, ILogger<MealManagementController> logger)
        {
            _mealManagementService = mealManagementService;
            _logger = logger;
        }

        [HttpPost("AddMealType")]
        public async Task<IActionResult> AddMealType([FromBody] AddMealTypeRequest request)
        {
            _logger.LogInformation("AddMealType Request Received: {@Request}", request);
            var result = await _mealManagementService.AddMealType(request);
            _logger.LogInformation("AddMealType Response: {@Response}", result);
            return Ok(result);
        }

        [HttpPost("GetMealType")]
        public async Task<IActionResult> GetMealType([FromBody] GetMealTypeRequest request)
        {
            _logger.LogInformation("GetMealType Request Received: {@Request}", request);
            var result = await _mealManagementService.GetMealType(request);
            _logger.LogInformation("GetMealType Response: {@Response}", result);
            return Ok(result);
        }

        [HttpPost("DeleteMealType")]
        public async Task<IActionResult> DeleteMealType([FromBody] DeleteMealTypeRequest request)
        {
            _logger.LogInformation("DeleteMealType Request Received: {@Request}", request);
            var result = await _mealManagementService.DeleteMealType(request);
            _logger.LogInformation("DeleteMealType Response: {@Response}", result);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("GetMealPlanner")]
        public async Task<IActionResult> GetMealPlanner([FromBody] GetMealPlannerRequest request)
        {
            _logger.LogInformation("GetMealPlanner Request Received: {@Request}", request);
            var result = await _mealManagementService.GetMealPlanner(request);
            _logger.LogInformation("GetMealPlanner Response: {@Response}", result);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("SetMealPlanner")]
        public async Task<IActionResult> SetMealPlanner([FromBody] SetMealPlannerRequest request)
        {
            var response = await _mealManagementService.SetMealPlanner(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetDailyMealMenu")]
        public async Task<IActionResult> GetDailyMealMenu([FromBody] GetDailyMealMenuRequest request)
        {
            _logger.LogInformation("GetDailyMealMenu Request Received: {@Request}", request);
            var result = await _mealManagementService.GetDailyMealMenu(request);
            _logger.LogInformation("GetDailyMealMenu Response: {@Response}", result);
            return StatusCode(result.StatusCode, result);
        }
    }
}
