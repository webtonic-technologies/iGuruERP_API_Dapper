using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Attendance_API.Services.Interfaces;
using Attendance_API.DTOs;
using System.Threading.Tasks;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class GeoFencingController : ControllerBase
    {
        private readonly IGeoFencingService _geoFencingService;

        public GeoFencingController(IGeoFencingService geoFencingService)
        {
            _geoFencingService = geoFencingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGeoFencings([FromQuery] GeoFencingQueryParams request)
        {
            var response = await _geoFencingService.GetAllGeoFencings(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGeoFencingById(int id)
        {
            var response = await _geoFencingService.GetGeoFencingById(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateGeoFencing(List<GeoFencingDTO> geoFencings)
        {

            var response = await _geoFencingService.AddOrUpdateGeoFencing(geoFencings);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(new ServiceResponse<bool>(true, "All GeoFencings processed successfully", true, 200));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGeoFencing(int id)
        {
            var response = await _geoFencingService.DeleteGeoFencing(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
    }
}

