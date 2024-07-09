using Microsoft.AspNetCore.Mvc;
using Transport_API.DTOs.Requests;
using Transport_API.Services.Interfaces;

namespace Transport_API.Controllers
{
    [Route("iGuru/Configuration/RoutePlan")]
    [ApiController]
    public class RoutePlanController : ControllerBase
    {
        private readonly IRoutePlanService _routePlanService;

        public RoutePlanController(IRoutePlanService routePlanService)
        {
            _routePlanService = routePlanService;
        }

        [HttpPost("AddUpdateRoutePlan")]
        public async Task<IActionResult> AddUpdateRoutePlan(RoutePlanRequestDTO request)
        {
            try
            {
                var response = await _routePlanService.AddUpdateRoutePlan(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GetAllRoutePlan")]
        public async Task<IActionResult> GetAllRoutePlan(GetAllRoutePlanRequest request)
        {
            try
            {
                var response = await _routePlanService.GetAllRoutePlans(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetRoutePlan/{RoutePlanId}")]
        public async Task<IActionResult> GetRoutePlan(int RoutePlanId)
        {
            try
            {
                var response = await _routePlanService.GetRoutePlanById(RoutePlanId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("Status/{RoutePlanId}")]
        public async Task<IActionResult> UpdateRoutePlanStatus(int RoutePlanId)
        {
            try
            {
                var response = await _routePlanService.UpdateRoutePlanStatus(RoutePlanId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
