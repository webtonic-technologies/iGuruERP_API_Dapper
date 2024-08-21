using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Lesson_API.Controllers
{
    [ApiController]  // Make sure there's only one of this attribute
    [Route("iGuru/LessonPlanning/[controller]")]
    public class ChapterSubtopicListController : ControllerBase  // Ensure this name is unique
    {
        private readonly IChapterSubtopicListService _chapterSubtopicListService;

        public ChapterSubtopicListController(IChapterSubtopicListService chapterSubtopicListService)
        {
            _chapterSubtopicListService = chapterSubtopicListService;
        }

        [HttpPost("GetChapterSubtopic")]
        public async Task<IActionResult> GetChapterSubtopic([FromBody] GetChapterSubtopicRequest request)
        {
            var response = await _chapterSubtopicListService.GetChapterSubtopics(request.CurriculumID, request.InstituteID);

            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }
    }
}
