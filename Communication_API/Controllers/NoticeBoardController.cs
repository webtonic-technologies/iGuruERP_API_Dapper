using Communication_API.DTOs.Requests.NoticeBoard;
using Communication_API.DTOs.ServiceResponse;
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

        [HttpPost]
        [Route("NoticeSetStudentView")]
        public async Task<IActionResult> NoticeSetStudentView([FromBody] NoticeSetStudentViewRequest request)
        {
            var response = await _noticeBoardService.NoticeSetStudentView(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost]
        [Route("NoticeSetEmployeeView")]
        public async Task<IActionResult> NoticeSetEmployeeView([FromBody] NoticeSetEmployeeViewRequest request)
        {
            var response = await _noticeBoardService.NoticeSetEmployeeView(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost]
        [Route("GetStudentNoticeStatistics")]
        public async Task<IActionResult> GetStudentNoticeStatistics([FromBody] GetStudentNoticeStatisticsRequest request)
        {
            var response = await _noticeBoardService.GetStudentNoticeStatistics(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetEmployeeNoticeStatistics")]
        public async Task<IActionResult> GetEmployeeNoticeStatistics([FromBody] GetEmployeeNoticeStatisticsRequest request)
        {
            var response = await _noticeBoardService.GetEmployeeNoticeStatistics(request);
            if (response.Success)
                return Ok(response);
            return StatusCode(response.StatusCode, response);
        }
        

        [HttpPost("DeleteNotice")]
        public async Task<IActionResult> DeleteNotice([FromBody] DeleteNoticeRequest request)
        {
            if (request == null || request.InstituteID <= 0 || request.NoticeID <= 0)
            {
                return BadRequest(new ServiceResponse<string>(false, "Invalid request parameters.", "Failure", 400));
            }

            var response = await _noticeBoardService.DeleteNotice(request.InstituteID, request.NoticeID);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }


        [HttpPost]
        [Route("DeleteCircular")]
        public async Task<IActionResult> DeleteCircular([FromBody] DeleteCircularRequest request)
        {
            var response = await _noticeBoardService.DeleteCircular(request);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

    }
}
