using Microsoft.AspNetCore.Mvc;
using Transport_API.DTOs.Requests;
using Transport_API.Services.Interfaces;

namespace Transport_API.Controllers
{
    [Route("iGuru/RouteAllocation/RouteMapping")]
    [ApiController]
    public class RouteMappingController : ControllerBase
    {
        private readonly IRouteMappingService _routeMappingService;

        public RouteMappingController(IRouteMappingService routeMappingService)
        {
            _routeMappingService = routeMappingService;
        }

        [HttpPost("AddUpdateRouteMapping")]
        public async Task<IActionResult> AddUpdateRouteMapping(RouteMapping request)
        {
            try
            {
                var response = await _routeMappingService.AddUpdateRouteMapping(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GetAllRouteMapping")]
        public async Task<IActionResult> GetAllRouteMapping(GetAllRouteMappingRequest request)
        {
            try
            {
                var response = await _routeMappingService.GetAllRouteMappings(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetRouteMapping/{RouteMappingId}")]
        public async Task<IActionResult> GetRouteMapping(int RouteMappingId)
        {
            try
            {
                var response = await _routeMappingService.GetRouteMappingById(RouteMappingId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("Status/{RouteMappingId}")]
        public async Task<IActionResult> UpdateRouteMappingStatus(int RouteMappingId)
        {
            try
            {
                var response = await _routeMappingService.UpdateRouteMappingStatus(RouteMappingId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
