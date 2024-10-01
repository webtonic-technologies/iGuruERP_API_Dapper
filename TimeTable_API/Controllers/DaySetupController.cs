using Microsoft.AspNetCore.Mvc;
using TimeTable_API.Services.Interfaces;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.ServiceResponse;
using Configuration_API.DTOs.Requests;

namespace TimeTable_API.Controllers
{
    [ApiController]
    [Route("iGuru/Configuration/DaySetup")]
    public class DaySetupController : ControllerBase
    {
        private readonly IDaySetupService _daySetupService;

        public DaySetupController(IDaySetupService daySetupService)
        {
            _daySetupService = daySetupService;
        }

        [HttpPost("GetAllDaySetups")]
        public async Task<IActionResult> GetAllDaySetups([FromBody] GetAllDaySetupsRequest request)
        {
            var response = await _daySetupService.GetAllDaySetups(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetDaySetupById/{planId}")]
        public async Task<IActionResult> GetDaySetupById(int planId)
        {
            var response = await _daySetupService.GetDaySetupById(planId);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("AddUpdatePlan")]
        public async Task<IActionResult> AddUpdatePlan([FromBody] AddUpdatePlanRequest request)
        {
            var response = await _daySetupService.AddUpdatePlan(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("DeleteDaySetup/{planId}")]
        public async Task<IActionResult> DeleteDaySetup(int planId)
        {
            var response = await _daySetupService.DeleteDaySetup(planId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
