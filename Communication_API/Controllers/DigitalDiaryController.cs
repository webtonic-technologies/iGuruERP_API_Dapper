using Communication_API.DTOs.Requests.DigitalDiary;
using Communication_API.Services.Interfaces.DigitalDiary;
using Microsoft.AspNetCore.Mvc;

namespace Communication_API.Controllers
{
    [Route("iGuru/DigitalDiary/[controller]")]
    [ApiController]
    public class DigitalDiaryController : ControllerBase
    {
        private readonly IDigitalDiaryService _digitalDiaryService;

        public DigitalDiaryController(IDigitalDiaryService digitalDiaryService)
        {
            _digitalDiaryService = digitalDiaryService;
        }

        [HttpPost("AddUpdateDiary")]
        public async Task<IActionResult> AddUpdateDiary([FromBody] AddUpdateDiaryRequest request)
        {
            var response = await _digitalDiaryService.AddUpdateDiary(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllDiary")]
        public async Task<IActionResult> GetAllDiary([FromBody] GetAllDiaryRequest request)
        {
            var response = await _digitalDiaryService.GetAllDiary(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("DeleteDiary/{DiaryID}")]
        public async Task<IActionResult> DeleteDiary(int DiaryID)
        {
            var response = await _digitalDiaryService.DeleteDiary(DiaryID);
            return StatusCode(response.StatusCode, response);
        }
    }
}
