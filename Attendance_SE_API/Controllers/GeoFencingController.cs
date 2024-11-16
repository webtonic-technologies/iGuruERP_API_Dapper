using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_SE_API.Controllers
{
    [Route("iGuru/Employee/Configuration")]
    [ApiController]
    public class GeoFencingController : ControllerBase
    {
        private readonly IGeoFencingRepository _geoFencingRepository;

        public GeoFencingController(IGeoFencingRepository geoFencingRepository)
        {
            _geoFencingRepository = geoFencingRepository;
        }

        //[HttpPost("AddGeoFancing")]
        //public async Task<IActionResult> AddGeoFancing([FromBody] GeoFencingRequest request)
        //{
        //    var response = await _geoFencingRepository.AddGeoFancing(request);
        //    return StatusCode(response.StatusCode, response);
        //}

        [HttpPost("AddGeoFancing")]
        public async Task<IActionResult> AddGeoFancing([FromBody] List<GeoFencingRequest> geoFencings)
        {
            if (geoFencings == null || !geoFencings.Any())
            {
                return BadRequest("Geo-fencing list is required.");
            }

            var response = await _geoFencingRepository.AddGeoFancing(geoFencings); // Call service method
            return StatusCode(response.StatusCode, response);
        }



        [HttpPost("GetAllGeoFancing")]
        public async Task<IActionResult> GetAllGeoFancing([FromBody] PaginationRequest request)
        {
            var response = await _geoFencingRepository.GetAllGeoFancing(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetGeoFancing/{geoFencingID}")]
        public async Task<IActionResult> GetGeoFancing(int geoFencingID)
        {
            var response = await _geoFencingRepository.GetGeoFancing(geoFencingID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("DeleteGeoFancing/{geoFencingID}")]
        public async Task<IActionResult> DeleteGeoFancing(int geoFencingID)
        {
            var response = await _geoFencingRepository.DeleteGeoFancing(geoFencingID);
            return StatusCode(response.StatusCode, response);
        }
    }
}

