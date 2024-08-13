using Lesson_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Lesson_API.Controllers
{
    [Route("iGuru/LessonPlanning/AssignmentType")]
    [ApiController]
    public class AssignmentTypeController : ControllerBase
    {
        private readonly IAssignmentTypeService _assignmentTypeService;

        public AssignmentTypeController(IAssignmentTypeService assignmentTypeService)
        {
            _assignmentTypeService = assignmentTypeService;
        }

        [HttpGet("GetAssignmentType_Fetch")]
        public async Task<IActionResult> GetAssignmentType_Fetch()
        {
            var response = await _assignmentTypeService.GetAllAssignmentTypes();
            return StatusCode(response.StatusCode, response);
        }
    }
}
