using Microsoft.AspNetCore.Mvc;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.Services.Interfaces;
using Attendance_SE_API.ServiceResponse;
using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Responses;

namespace Attendance_SE_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class GeoFencingEntryController : ControllerBase
    {
        private readonly IGeoFencingEntryService _geoFencingEntryService;

        public GeoFencingEntryController(IGeoFencingEntryService geoFencingEntryService)
        {
            _geoFencingEntryService = geoFencingEntryService;
        }

        [HttpPost("AddGeoFencingEntry")]
        public async Task<ActionResult<ServiceResponse<string>>> AddGeoFencingEntry([FromBody] GeoFencingEntryRequest request)
        {
            var response = await _geoFencingEntryService.AddGeoFencingEntry(request);
            return Ok(response);
        }

        [HttpPost("GetGeoFencingEntry")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<GeoFencingEntryResponse>>>> GetGeoFencingEntry([FromBody] GeoFencingEntryRequest2 request)
        {
            var response = await _geoFencingEntryService.GetGeoFencingEntry(request);
            return Ok(response);
        }


        //[HttpPost("GetGeoFencingEntry")]
        //public async Task<ActionResult<ServiceResponse<IEnumerable<GeoFencingEntryResponse>>>> GetGeoFencingEntry([FromBody] GeoFencingEntryRequest2 request)
        //{
        //    var response = await _geoFencingEntryService.GetGeoFencingEntry(request);
        //    return Ok(response);
        //}
    }
}
