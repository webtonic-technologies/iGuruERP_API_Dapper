using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.Services.Interfaces;

namespace TimeTable_API.Controllers
{
    [ApiController]
    [Route("iGuru/Employee/Workload")]
    public class EmployeeWorkloadController : ControllerBase
    {
        private readonly IEmployeeWorkloadService _service;

        public EmployeeWorkloadController(IEmployeeWorkloadService service)
        {
            _service = service;
        }

        [HttpPost("Get")]
        public async Task<IActionResult> GetEmployeeWorkload([FromBody] EmployeeWorkloadRequest request)
        {
            var result = await _service.GetEmployeeWorkload(request);
            return Ok(result);
        }

        [HttpPost("AddUpdateWorkLoad")]
        public async Task<IActionResult> AddUpdateWorkLoad([FromBody] AddUpdateWorkloadRequest request)
        {
            var response = await _service.AddUpdateWorkload(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
