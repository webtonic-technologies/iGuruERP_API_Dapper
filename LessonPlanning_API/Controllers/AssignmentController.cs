using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Lesson_API.Controllers
{
    [Route("iGuru/Assignment/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpPost("AddUpdateAssignment")]
        public async Task<IActionResult> AddUpdateAssignment([FromBody] AssignmentRequest request)
        {
            var response = await _assignmentService.AddUpdateAssignment(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetAllAssignments")]
        public async Task<IActionResult> GetAllAssignments([FromBody] GetAllAssignmentsRequest request)
        {
            var response = await _assignmentService.GetAllAssignments(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetAssignment/{id}")]
        public async Task<IActionResult> GetAssignment(int id)
        {
            var response = await _assignmentService.GetAssignmentById(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut("DeleteAssignment/{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var response = await _assignmentService.DeleteAssignment(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
