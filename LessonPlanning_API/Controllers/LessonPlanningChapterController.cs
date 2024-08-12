using Lesson_API.DTOs.Requests;
using Lesson_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Lesson_API.Controllers
{
    [Route("iGuru/LessonPlanning/LessonPlanningChapter")]
    [ApiController]
    public class LessonPlanningChapterController : ControllerBase
    {
        private readonly ILessonPlanningChapterService _lessonPlanningChapterService;

        public LessonPlanningChapterController(ILessonPlanningChapterService lessonPlanningChapterService)
        {
            _lessonPlanningChapterService = lessonPlanningChapterService;
        }

        [HttpPost("GetLessonPlanningChapter_Fetch")]
        public async Task<IActionResult> GetLessonPlanningChapter_Fetch([FromBody] LessonPlanningChapterRequest request)
        {
            var response = await _lessonPlanningChapterService.GetChaptersByInstituteId(request.InstituteID);
            if (response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return StatusCode(response.StatusCode, response);
        }
    }
}
