using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Services.Interfaces;
using Lesson_API.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Lesson_API.Controllers
{
    [Route("iGuru/LessonPlanning/[controller]")]
    [ApiController]
    public class LessonPlanningController : ControllerBase
    {
        private readonly ILessonPlanningService _lessonPlanningService;

        public LessonPlanningController(ILessonPlanningService lessonPlanningService)
        {
            _lessonPlanningService = lessonPlanningService;
        }

        [HttpPost("AddUpdateLessonPlanning")]
        public async Task<IActionResult> AddUpdateLessonPlanning([FromBody] LessonPlanningRequest request)
        {
            var response = await _lessonPlanningService.AddUpdateLessonPlanning(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetAllLessonPlanning")]
        public async Task<IActionResult> GetAllLessonPlanning([FromBody] GetAllLessonPlanningRequest request)
        {
            var response = await _lessonPlanningService.GetAllLessonPlanning(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetLessonPlanning/{id}")]
        public async Task<IActionResult> GetLessonPlanning(int id)
        {
            var response = await _lessonPlanningService.GetLessonPlanningById(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut("DeleteLessonPlanning/{id}")]
        public async Task<IActionResult> DeleteLessonPlanning(int id)
        {
            var response = await _lessonPlanningService.DeleteLessonPlanning(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetLessonPlanning")]
        public async Task<IActionResult> GetLessonPlanning([FromBody] GetLessonPlanningRequest request)
        {
            var response = await _lessonPlanningService.GetLessonPlanning(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetLessonStatus")]
        public async Task<IActionResult> GetLessonStatus()
        {
            var response = await _lessonPlanningService.GetLessonStatus();
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
