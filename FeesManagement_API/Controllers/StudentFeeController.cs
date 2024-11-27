using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Services.Interfaces;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/FeeAssignments/IndividualStudentFees")]
    [ApiController]
    public class StudentFeeController : ControllerBase
    {
        private readonly IStudentFeeService _studentFeeService;

        public StudentFeeController(IStudentFeeService studentFeeService)
        {
            _studentFeeService = studentFeeService;
        }

        [HttpPost("GetStudentFees")]
        public IActionResult GetStudentFees([FromBody] StudentFeeRequest request)
        {
            var response = _studentFeeService.GetStudentFees(request);
            return Ok(response);
        }
    }
}
