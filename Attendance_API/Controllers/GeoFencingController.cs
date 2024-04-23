using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Attendance_API.Services.Interfaces;
using Attendance_API.DTOs;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetAllGeoFencings()
        {
            var response = await _geoFencingService.GetAllGeoFencings();
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
        public async Task<IActionResult> AddGeoFencing(GeoFencingDTO geoFencing)
        {
            var response = await _geoFencingService.AddGeoFencing(geoFencing);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGeoFencing(GeoFencingDTO geoFencing)
        {
            var response = await _geoFencingService.UpdateGeoFencing(geoFencing);
            if (response.Success)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
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


