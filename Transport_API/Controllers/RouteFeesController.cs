using Microsoft.AspNetCore.Mvc;
using Transport_API.DTOs.Requests;
using Transport_API.Services.Interfaces;

namespace Transport_API.Controllers
{
    [Route("iGuru/RouteAllocation/RouteMapping")]
    [ApiController]
    public class RouteFeesController : ControllerBase
    {
        private readonly IRouteFeesServices _routeFeesServices;

        public RouteFeesController(IRouteFeesServices routeFeesServices)
        {
            _routeFeesServices = routeFeesServices;
        }

        [HttpPost("AddUpdateRouteFeeStructure")]
        public async Task<IActionResult> AddUpdateRouteFeeStructure(RouteFeeStructure request)
        {
            try
            {
                var response = await _routeFeesServices.AddUpdateRouteFeeStructure(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetRouteFeeStructureById/{routeFeeStructureId}")]
        public async Task<IActionResult> GetRouteFeeStructureById(int routeFeeStructureId)
        {
            try
            {
                var response = await _routeFeesServices.GetRouteFeeStructureById(routeFeeStructureId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
