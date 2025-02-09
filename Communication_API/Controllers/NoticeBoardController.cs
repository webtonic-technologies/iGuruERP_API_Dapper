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

        [HttpPost]
        [Route("CircularSetStudentView")]
        public async Task<IActionResult> CircularSetStudentView([FromBody] CircularSetStudentViewRequest request)
        {
            var response = await _noticeBoardService.CircularSetStudentView(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost]
        [Route("CircularSetEmployeeView")]
        public async Task<IActionResult> CircularSetEmployeeView([FromBody] CircularSetEmployeeViewRequest request)
        {
            var response = await _noticeBoardService.CircularSetEmployeeView(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost]
        [Route("GetStudentCircularStatistics")]
        public async Task<IActionResult> GetStudentCircularStatistics([FromBody] GetStudentCircularStatisticsRequest request)
        {
            var response = await _noticeBoardService.GetStudentCircularStatistics(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetEmployeeCircularStatistics")]
        public async Task<IActionResult> GetEmployeeCircularStatistics([FromBody] GetEmployeeCircularStatisticsRequest request)
        {
            var response = await _noticeBoardService.GetEmployeeCircularStatistics(request);
            if (response.Success)
                return Ok(response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllNoticeExport")]
        public async Task<IActionResult> GetAllNoticeExport([FromBody] GetAllNoticeExportRequest request)
        {
            // Get the export file path from the service
            var response = await _noticeBoardService.GetAllNoticeExport(request);
            if (response.Success)
            {
                string filePath = response.Data;
                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    string mimeType;
                    string fileName;
                    if (request.ExportType == 1)
                    {
                        mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        fileName = "NoticeReport.xlsx";
                    }
                    else if (request.ExportType == 2)
                    {
                        mimeType = "text/csv";
                        fileName = "NoticeReport.csv";
                    }
                    else
                    {
                        return BadRequest("Invalid ExportType.");
                    }
                    return File(fileBytes, mimeType, fileName);
                }
                else
                {
                    return BadRequest("File not found.");
                }
            }
            else
            {
                return BadRequest(response.Message);
            }
        }



        [HttpPost("GetAllCircularExport")]
        public async Task<IActionResult> GetAllCircularExport([FromBody] GetAllCircularExportRequest request)
        {
            var response = await _noticeBoardService.GetAllCircularExport(request);
            if (response.Success)
            {
                string filePath = response.Data;
                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    string mimeType;
                    string fileName;
                    if (request.ExportType == 1)
                    {
                        mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        fileName = "CircularReport.xlsx";
                    }
                    else if (request.ExportType == 2)
                    {
                        mimeType = "text/csv";
                        fileName = "CircularReport.csv";
                    }
                    else
                    {
                        return BadRequest("Invalid ExportType.");
                    }
                    return File(fileBytes, mimeType, fileName);
                }
                else
                {
                    return BadRequest("File not found.");
                }
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
    }
}
