using Lesson_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Lesson_API.Controllers
{
    [Route("iGuru/LessonPlanning/HomeWorkType")]
    [ApiController]
    public class HomeWorkTypeController : ControllerBase
    {
        private readonly IHomeWorkTypeService _homeWorkTypeService;

        public HomeWorkTypeController(IHomeWorkTypeService homeWorkTypeService)
        {
            _homeWorkTypeService = homeWorkTypeService;
        }

        [HttpGet("GetHomeWorkType_Fetch")]
        public async Task<IActionResult> GetHomeWorkType_Fetch()
        {
            var response = await _homeWorkTypeService.GetAllHomeWorkTypes();
            return StatusCode(response.StatusCode, response);
        }
    }
}
