using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.Services.Interfaces;

namespace TimeTable_API.Controllers
{
    [ApiController]
    [Route("iGuru/TimeTable/EmployeeWise")]
    public class EmployeeWiseController : ControllerBase
    {
        private readonly IEmployeeWiseService _service;

        public EmployeeWiseController(IEmployeeWiseService service)
        {
            _service = service;
        }

        [HttpPost("Get")]
        public async Task<IActionResult> GetEmployeeWiseTimetable([FromBody] EmployeeWiseRequest request)
        {
            var response = await _service.GetEmployeeWiseTimetable(request);

            if (response.Success)
                return Ok(response);
            else
                return StatusCode(response.StatusCode, response);
        }
    }
}
