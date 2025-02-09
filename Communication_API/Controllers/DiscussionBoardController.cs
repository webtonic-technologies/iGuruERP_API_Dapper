using Communication_API.DTOs.Requests.DiscussionBoard;
using Communication_API.Services.Interfaces.DiscussionBoard;
using Communication_API.DTOs.Responses.DiscussionBoard;
using Communication_API.DTOs.ServiceResponse;


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
            ServiceResponse<List<GetAllDiscussionResponse>> response = await _discussionBoardService.GetAllDiscussion(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetDiscussionBoard/{DiscussionBoardID}")]
        public async Task<IActionResult> GetDiscussionBoardDetails([FromRoute] int DiscussionBoardID, [FromQuery] int InstituteID)
        {
            var request = new GetDiscussionBoardDetailsRequest
            {
                DiscussionBoardID = DiscussionBoardID,
                InstituteID = InstituteID
            };

            ServiceResponse<GetDiscussionBoardDetailsResponse> response =
                await _discussionBoardService.GetDiscussionBoardDetails(request);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("DeleteDiscussion/{DiscussionBoardID}")]
        public async Task<IActionResult> DeleteDiscussion(int DiscussionBoardID)
        {
            var response = await _discussionBoardService.DeleteDiscussion(DiscussionBoardID);
            return StatusCode(response.StatusCode, response);
        }
         

        //[HttpPost("CreateDiscussionThread")]
        //public async Task<IActionResult> CreateDiscussionThread([FromBody] CreateDiscussionThreadRequest request)
        //{
        //    var response = await _discussionBoardService.CreateDiscussionThread(request);
        //    return StatusCode(response.StatusCode, response);
        //}

        //[HttpPost("GetDiscussionThread/{DiscussionBoardID}")]
        //public async Task<IActionResult> GetDiscussionThread(int DiscussionBoardID)
        //{
        //    var response = await _discussionBoardService.GetDiscussionThread(DiscussionBoardID);
        //    return StatusCode(response.StatusCode, response);
        //}

        [HttpPost("AddDiscussionBoardComment")]
        public async Task<IActionResult> AddDiscussionBoardComment([FromBody] AddDiscussionBoardCommentRequest request)
        {
            ServiceResponse<string> response = await _discussionBoardService.AddDiscussionBoardComment(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("AddDiscussionBoardReaction")]
        public async Task<IActionResult> AddDiscussionBoardReaction([FromBody] AddDiscussionBoardReactionRequest request)
        {
            ServiceResponse<string> response = await _discussionBoardService.AddDiscussionBoardReaction(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetDiscussionBoardComments")]
        public async Task<IActionResult> GetDiscussionBoardComments([FromBody] GetDiscussionBoardCommentsRequest request)
        {
            ServiceResponse<List<GetDiscussionBoardCommentsResponse>> response =
                await _discussionBoardService.GetDiscussionBoardComments(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetDiscussionBoardReactions")]
        public async Task<IActionResult> GetDiscussionBoardReactions([FromBody] GetDiscussionBoardReactionsRequest request)
        {
            ServiceResponse<GetDiscussionBoardReactionsResponse> response = await _discussionBoardService.GetDiscussionBoardReactions(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
