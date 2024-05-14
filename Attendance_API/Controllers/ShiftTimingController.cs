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

        [HttpPost]
        public async Task<IActionResult> AddShiftTimingAndDesignations([FromBody] ShiftTimingRequestDTO request)
        {
            var result = await _shiftTimingService.AddShiftTimingAndDesignations(request);
            if (result)
                return Ok();
            else
                return BadRequest("Failed to add shift timing and designations.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShiftTimings()
        {
            var result = await _shiftTimingService.GetAllShiftTimings();
            if (result != null)
                return Ok(result);
            else
                return NotFound("No shift timings found.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShiftTimingById(int id)
        {
            var result = await _shiftTimingService.GetShiftTimingById(id);
            if (result != null)
                return Ok(result);
            else
                return NotFound($"Shift timing with ID {id} not found.");
        }

        [HttpPut]
        public async Task<IActionResult> EditShiftTimingAndDesignations([FromBody] ShiftTimingRequestDTO request)
        {
            var result = await _shiftTimingService.EditShiftTimingAndDesignations(request);
            if (result)
                return Ok();
            else
                return BadRequest("Failed to edit shift timing and designations.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShiftTiming(int id)
        {
            var result = await _shiftTimingService.DeleteShiftTiming(id);
            if (result)
                return Ok();
            else
                return BadRequest($"Failed to delete shift timing with ID {id}.");
        }
    }
}
