using Lesson_API.DTOs.Requests;
using Lesson_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Lesson_API.Controllers
{
    [Route("iGuru/Curriculum/[controller]")]
    [ApiController]
    public class CurriculumController : ControllerBase
    {
        private readonly ICurriculumService _curriculumService;

        public CurriculumController(ICurriculumService curriculumService)
        {
            _curriculumService = curriculumService;
        }

        [HttpPost("AddUpdateCurriculum")]
        public async Task<IActionResult> AddUpdateCurriculum([FromBody] CurriculumRequest request)
        {
            var response = await _curriculumService.AddUpdateCurriculum(request);
            return StatusCode(response.StatusCode, response);  // Ensure the correct status code is returned
        }

        [HttpPost("GetAllCurriculum")]
        public async Task<IActionResult> GetAllCurriculum([FromBody] GetAllCurriculumRequest request)
        {
            var response = await _curriculumService.GetAllCurriculum(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpGet("GetCurriculum/{CurriculumID}")]
        public async Task<IActionResult> GetCurriculum(int CurriculumID)
        {
            var response = await _curriculumService.GetCurriculumById(CurriculumID);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("Delete/{CurriculumID}")]
        public async Task<IActionResult> DeleteCurriculum(int CurriculumID)
        {
            var response = await _curriculumService.DeleteCurriculum(CurriculumID);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
