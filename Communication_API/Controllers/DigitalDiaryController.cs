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

        [HttpPost("GetAllDiaryExport")]
        public async Task<IActionResult> GetAllDiaryExport([FromBody] GetAllDiaryExportRequest request)
        {
            var response = await _digitalDiaryService.GetAllDiaryExport(request);

            if (response.Success)
            {
                var fileName = $"DigitalDiaryExport{(request.ExportType == 1 ? ".xlsx" : ".csv")}";
                return File(response.Data, response.StatusCode == 200 ?
                    (request.ExportType == 1 ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv") :
                    "application/octet-stream", fileName);
            }

            return BadRequest(response.Message);
        }
    }
}
