using Communication_API.DTOs.Requests.DiscussionBoard;
using Communication_API.Services.Interfaces.DiscussionBoard;
using Microsoft.AspNetCore.Mvc;

namespace Communication_API.Controllers
{
    [Route("iGuru/DiscussionBoard/[controller]")]
    [ApiController]
    public class DiscussionBoardController : ControllerBase
    {
        private readonly IDiscussionBoardService _discussionBoardService;

        public DiscussionBoardController(IDiscussionBoardService discussionBoardService)
        {
            _discussionBoardService = discussionBoardService;
        }

        [HttpPost("AddUpdateDiscussion")]
        public async Task<IActionResult> AddUpdateDiscussion([FromBody] AddUpdateDiscussionRequest request)
        {
            var response = await _discussionBoardService.AddUpdateDiscussion(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllDiscussion")]
        public async Task<IActionResult> GetAllDiscussion([FromBody] GetAllDiscussionRequest request)
        {
            var response = await _discussionBoardService.GetAllDiscussion(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("DeleteDiscussion/{DiscussionBoardID}")]
        public async Task<IActionResult> DeleteDiscussion(int DiscussionBoardID)
        {
            var response = await _discussionBoardService.DeleteDiscussion(DiscussionBoardID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetDiscussionBoard/{DiscussionBoardID}")]
        public async Task<IActionResult> GetDiscussionBoard(int DiscussionBoardID)
        {
            var response = await _discussionBoardService.GetDiscussionBoard(DiscussionBoardID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("CreateDiscussionThread")]
        public async Task<IActionResult> CreateDiscussionThread([FromBody] CreateDiscussionThreadRequest request)
        {
            var response = await _discussionBoardService.CreateDiscussionThread(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetDiscussionThread/{DiscussionBoardID}")]
        public async Task<IActionResult> GetDiscussionThread(int DiscussionBoardID)
        {
            var response = await _discussionBoardService.GetDiscussionThread(DiscussionBoardID);
            return StatusCode(response.StatusCode, response);
        }
    }
}
