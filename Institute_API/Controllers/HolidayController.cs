using Institute_API.DTOs;
using Institute_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Institute_API.Controllers
{
    [Route("iGuru/[controller]")]
    [ApiController]
    public class HolidayController : ControllerBase
    {
        private readonly IHolidayService _holidayService;

        public HolidayController(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        [HttpPost("AddUpdateHoliday")]
        public async Task<IActionResult> AddUpdateHoliday([FromBody] HolidayRequestDTO holidayDTO)
        {
            try
            {
                var response = await _holidayService.AddUpdateHoliday(holidayDTO);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetHolidayById")]
        public async Task<IActionResult> GetHolidayById(int holidayId)
        {
            try
            {
                var response = await _holidayService.GetHolidayById(holidayId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllHolidays/{Institute_id}")]
        public async Task<IActionResult> GetAllHolidays(int Institute_id)
        {
            try
            {
                var response = await _holidayService.GetAllHolidays(Institute_id);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetApprovedHolidays/{Institute_id}")]
        public async Task<IActionResult> GetApprovedHolidays(int Institute_id)
        {
            try
            {
                var response = await _holidayService.GetApprovedHolidays(Institute_id);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteHoliday")]
        public async Task<IActionResult> DeleteHoliday(int holidayId)
        {
            try
            {
                var response = await _holidayService.DeleteHoliday(holidayId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UpdateHolidayApprovalStatus")]
        public async Task<IActionResult> UpdateHolidayApprovalStatus([FromBody] UpdateHolidayApprovalStatusRequest request)
        {
            try
            {
                var response = await _holidayService.UpdateHolidayApprovalStatus(request.holidayId, request.IsApproved, request.ApprovedBy);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
