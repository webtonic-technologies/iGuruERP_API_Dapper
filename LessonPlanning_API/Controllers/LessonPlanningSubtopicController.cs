using Lesson_API.DTOs.Requests;
using Lesson_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Lesson_API.Controllers
{
    [Route("iGuru/LessonPlanning/LessonPlanningSubtopic")]
    [ApiController]
    public class LessonPlanningSubtopicController : ControllerBase
    {
        private readonly ILessonPlanningSubtopicService _lessonPlanningSubtopicService;

        public LessonPlanningSubtopicController(ILessonPlanningSubtopicService lessonPlanningSubtopicService)
        {
            _lessonPlanningSubtopicService = lessonPlanningSubtopicService;
        }

        [HttpPost("GetLessonPlanningSubtopic_Fetch")]
        public async Task<IActionResult> GetLessonPlanningSubtopic_Fetch([FromBody] LessonPlanningSubtopicRequest request)
        {
            var response = await _lessonPlanningSubtopicService.GetSubtopicsByInstituteId(request.InstituteID);
            return StatusCode(response.StatusCode, response);
        }
    }
}
