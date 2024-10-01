using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.Services.Interfaces;

namespace TimeTable_API.Controllers
{
    [ApiController]
    [Route("iGuru/Configuration/TimeTable")]
    public class TimeTableController : ControllerBase
    {
        private readonly ITimeTableService _timeTableService;

        public TimeTableController(ITimeTableService timeTableService)
        {
            _timeTableService = timeTableService;
        }

        [HttpPost("AddUpdateTimeTable")]
        public async Task<IActionResult> AddUpdateTimeTable([FromBody] AddUpdateTimeTableRequest request)
        {
            var response = await _timeTableService.AddUpdateTimeTable(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllTimeTables")]
        public async Task<IActionResult> GetAllTimeTables([FromBody] GetAllTimeTablesRequest request)
        {
            var result = await _timeTableService.GetAllTimeTables(request);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(result.StatusCode, result.Message);
            }
        }

    }
}
