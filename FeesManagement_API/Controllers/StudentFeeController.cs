using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse; // Add this using directive for ServiceResponse
using FeesManagement_API.Services.Interfaces;
using System.Collections.Generic; // Add this using directive for List<T>

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

        [HttpPost]
        [Route("GetStudentFees")]
        public ActionResult<ServiceResponse<List<StudentFeeResponse>>> GetStudentFees([FromBody] StudentFeeRequest request)
        {
            var response = _studentFeeService.GetStudentFees(request);
            return Ok(response);
        }
    }
}
