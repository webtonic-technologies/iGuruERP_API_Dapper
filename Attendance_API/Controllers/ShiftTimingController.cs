using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Attendance_API.DTOs;
using Attendance_API.Services.Interfaces;
using System.Threading.Tasks;

namespace Attendance_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class ShiftTimingController : ControllerBase
    {
        private readonly IShiftTimingService _shiftTimingService;

        public ShiftTimingController(IShiftTimingService shiftTimingService)
        {
            _shiftTimingService = shiftTimingService;
        }

        [HttpPost("AddOrEditShiftTimingsAndDesignations")]
        public async Task<IActionResult> AddOrEditShiftTimingsAndDesignations([FromBody] List<ShiftTimingRequestDTO> request)
        {
            var result = await _shiftTimingService.AddOrEditShiftTimingsAndDesignations(request );
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("GetAllShiftTimings")]
        public async Task<IActionResult> GetAllShiftTimings([FromQuery]ShiftTimingFilterDTO request)
        {
            var result = await _shiftTimingService.GetAllShiftTimings(request);
            if (result.Success)
                return Ok(result);
            else
                return NotFound(result);
        }

        [HttpGet("GetShiftTimingById/{id}")]
        public async Task<IActionResult> GetShiftTimingById(int id)
        {
            var result = await _shiftTimingService.GetShiftTimingById(id);
            if (result.Success)
                return Ok(result);
            else
                return NotFound(result);
        }

        [HttpPut("EditShiftTimingAndDesignations")]
        public async Task<IActionResult> EditShiftTimingAndDesignations([FromBody] ShiftTimingRequestDTO request)
        {
            var result = await _shiftTimingService.AddOrEditShiftTimingsAndDesignations(new List<ShiftTimingRequestDTO> { request });
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpDelete("DeleteShiftTiming/{id}")]
        public async Task<IActionResult> DeleteShiftTiming(int id)
        {
            var result = await _shiftTimingService.DeleteShiftTiming(id);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}
