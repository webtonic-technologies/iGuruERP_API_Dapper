using Communication_API.DTOs.Requests.NoticeBoard;
using Communication_API.Services.Interfaces.NoticeBoard;
using Microsoft.AspNetCore.Mvc;

namespace Communication_API.Controllers
{
    [Route("iGuru/NoticeBoard/[controller]")]
    [ApiController]
    public class NoticeBoardController : ControllerBase
    {
        private readonly INoticeBoardService _noticeBoardService;

        public NoticeBoardController(INoticeBoardService noticeBoardService)
        {
            _noticeBoardService = noticeBoardService;
        }

        [HttpPost("AddUpdateNotice")]
        public async Task<IActionResult> AddUpdateNotice([FromBody] AddUpdateNoticeRequest request)
        {
            var response = await _noticeBoardService.AddUpdateNotice(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllNotice")]
        public async Task<IActionResult> GetAllNotice([FromBody] GetAllNoticeRequest request)
        {
            var response = await _noticeBoardService.GetAllNotice(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("AddUpdateCircular")]
        public async Task<IActionResult> AddUpdateCircular([FromBody] AddUpdateCircularRequest request)
        {
            var response = await _noticeBoardService.AddUpdateCircular(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllCircular")]
        public async Task<IActionResult> GetAllCircular([FromBody] GetAllCircularRequest request)
        {
            var response = await _noticeBoardService.GetAllCircular(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
