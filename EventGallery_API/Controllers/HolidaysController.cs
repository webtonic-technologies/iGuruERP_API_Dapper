using Microsoft.AspNetCore.Mvc;
using EventGallery_API.DTOs.Requests;
using EventGallery_API.Services.Interfaces;
using EventGallery_API.DTOs.Responses;

namespace EventGallery_API.Controllers
{
    [Route("iGuru/Holidays")]
    [ApiController]
    public class HolidaysController : ControllerBase
    {
        private readonly IHolidayService _holidayService;

        public HolidaysController(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }
        [HttpPost("AddUpdateHoliday")]
        public async Task<IActionResult> AddUpdateHoliday([FromBody] HolidayRequest request)
        {
            try
            {
                // Convert the string date in DD-MM-YYYY format to DateTime
                DateTime fromDate = DateTime.ParseExact(request.FromDate, "dd-MM-yyyy", null);
                DateTime toDate = DateTime.ParseExact(request.ToDate, "dd-MM-yyyy", null);

                // Convert back to standard ISO format for the database
                request.FromDate = fromDate.ToString("yyyy-MM-dd");
                request.ToDate = toDate.ToString("yyyy-MM-dd");

                // Call the service layer with the updated request
                var holidayID = await _holidayService.AddUpdateHoliday(request);

                var response = new
                {
                    success = true,
                    message = request.HolidayID == 0 ? "Holiday added successfully." : "Holiday updated successfully.",
                    data = holidayID,
                    statusCode = 200
                };

                return Ok(response);
            }
            catch (FormatException ex)
            {
                return BadRequest(new { success = false, message = "Invalid date format. Please use DD-MM-YYYY.", statusCode = 400 });
            }
        }


        [HttpPost("GetAllHolidays")]
        public async Task<IActionResult> GetAllHolidays([FromBody] HolidaySearchRequest request)
        {
            // Call the service layer to fetch the holidays
            var holidays = await _holidayService.GetAllHolidays(request);

            // Return the response with the holidays data
            var response = new
            {
                success = true,
                message = holidays.Any() ? "Holidays fetched successfully." : "No holidays found.",
                data = holidays,
                statusCode = 200
            };

            return Ok(response);
        }



        [HttpGet("GetHoliday/{HolidayID:int}")]
        public async Task<IActionResult> GetHoliday(int HolidayID)
        {
            if (HolidayID <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid HolidayID. It must be a positive integer.",
                    statusCode = 400
                });
            }

            var holiday = await _holidayService.GetHoliday(HolidayID);

            if (holiday == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Holiday not found.",
                    statusCode = 404
                });
            }

            var response = new
            {
                success = true,
                message = "Holiday fetched successfully.",
                data = new List<HolidayResponse> { holiday },
                statusCode = 200
            };

            return Ok(response);
        }


        [HttpDelete("DeleteHoliday/{HolidayID:int}")]
        public async Task<IActionResult> DeleteHoliday(int HolidayID)
        {
            var result = await _holidayService.DeleteHoliday(HolidayID);

            if (!result.Success) // Check if the deletion was successful
            {
                return NotFound(new
                {
                    success = false,
                    message = "Holiday not found.",
                    statusCode = 404
                });
            }

            return Ok(new
            {
                success = true,
                message = "Holiday deleted successfully.",
                statusCode = 200
            });
        }

         
    }
}


