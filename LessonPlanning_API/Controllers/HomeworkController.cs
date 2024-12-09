using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Lesson_API.Controllers
{
    [Route("iGuru/Homework/[controller]")]
    [ApiController]
    public class HomeworkController : ControllerBase
    {
        private readonly IHomeworkService _homeworkService;

        public HomeworkController(IHomeworkService homeworkService)
        {
            _homeworkService = homeworkService;
        }

        [HttpPost("AddUpdateHomework")]
        public async Task<IActionResult> AddUpdateHomework([FromBody] HomeworkRequest request)
        {
            var response = await _homeworkService.AddUpdateHomework(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetAllHomework")]
        public async Task<IActionResult> GetAllHomework([FromBody] GetAllHomeworkRequest request)
        {
            var response = await _homeworkService.GetAllHomework(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        //[HttpGet("GetHomework/{id}")]
        //public async Task<IActionResult> GetHomework(int id)
        //{
        //    var response = await _homeworkService.GetHomeworkById(id);
        //    if (response.Success)
        //    {
        //        return Ok(response);
        //    }
        //    return NotFound(response);
        //}

        [HttpPut("DeleteHomework/{id}")]
        public async Task<IActionResult> DeleteHomework(int id)
        {
            var response = await _homeworkService.DeleteHomework(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetHomeworkHistory")]
        public async Task<IActionResult> GetHomeworkHistory([FromBody] GetHomeworkHistoryRequest request)
        {
            var response = await _homeworkService.GetHomeworkHistory(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
