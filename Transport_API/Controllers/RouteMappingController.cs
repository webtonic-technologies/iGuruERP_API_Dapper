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

        [HttpDelete("Delete/{RouteMappingId}")]
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

        //[HttpPost("AddUpdateEmployeeStopMapping")]
        //public async Task<IActionResult> AddUpdateEmployeeStopMapping(List<EmployeeStopMapping> request)
        //{
        //    try
        //    {
        //        var response = await _routeMappingService.AddUpdateEmployeeStopMapping(request);
        //        return StatusCode(response.StatusCode, response);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        //[HttpPost("AddUpdateStudentStopMapping")]
        //public async Task<IActionResult> AddUpdateStudentStopMapping(List<StudentStopMapping> request)
        //{
        //    try
        //    {
        //        var response = await _routeMappingService.AddUpdateStudentStopMapping(request);
        //        return StatusCode(response.StatusCode, response);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        //[HttpGet("GetEmployeeStopMappings/{RouteMappingId}")]
        //public async Task<IActionResult> GetEmployeeStopMappings(int RoutePlanId)
        //{
        //    try
        //    {
        //        var response = await _routeMappingService.GetEmployeeStopMappings(RoutePlanId);
        //        return StatusCode(response.StatusCode, response);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        //[HttpGet("GetStudentStopMappings/{RouteMappingId}")]
        //public async Task<IActionResult> GetStudentStopMappings(int RoutePlanId)
        //{
        //    try
        //    {
        //        var response = await _routeMappingService.GetStudentStopMappings(RoutePlanId);
        //        return StatusCode(response.StatusCode, response);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        [HttpPost("RemoveEmployeeStopMapping")]
        public async Task<IActionResult> RemoveEmployeeStopMapping(List<EmployeeStopMapping> request)
        {
            try
            {
                var response = await _routeMappingService.RemoveEmployeeStopMapping(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("RemoveStudentStopMapping")]
        public async Task<IActionResult> RemoveStudentStopMapping(List<StudentStopMapping> request)
        {
            try
            {
                var response = await _routeMappingService.RemoveStudentStopMapping(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GetRouteVehicleDriverInfo")]
        public async Task<IActionResult> GetRouteVehicleDriverInfo([FromBody] RouteVehicleDriverInfoRequest request)
        {
            var response = await _routeMappingService.GetRouteVehicleDriverInfo(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetStudentsForRouteMapping")]
        public async Task<IActionResult> GetStudentsForRouteMapping([FromBody] GetStudentsForRouteMappingRequest request)
        {
            var response = await _routeMappingService.GetStudentsForRouteMapping(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetEmployeesForRouteMapping")]
        public async Task<IActionResult> GetEmployeesForRouteMapping([FromBody] GetEmployeesForRouteMappingRequest request)
        {
            var response = await _routeMappingService.GetEmployeesForRouteMapping(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}