using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.Services.Interfaces;

namespace TimeTable_API.Controllers
{
    [ApiController]
    [Route("iGuru/Employee/Substitution")]
    public class EmployeeSubstitutionController : ControllerBase
    {
        private readonly IEmployeeSubstitutionService _substitutionService;

        public EmployeeSubstitutionController(IEmployeeSubstitutionService substitutionService)
        {
            _substitutionService = substitutionService;
        }

        [HttpPost("Get")]
        public async Task<IActionResult> GetSubstitution([FromBody] EmployeeSubstitutionRequest request)
        {
            var result = await _substitutionService.GetSubstitution(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost("Update")]
        public async Task<IActionResult> UpdateSubstitution([FromBody] EmployeeSubstitutionRequest_Update request)
        {
            var response = await _substitutionService.UpdateSubstitution(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
