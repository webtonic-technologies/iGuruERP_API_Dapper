using Microsoft.AspNetCore.Mvc;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.Services.Interfaces;

namespace StudentManagement_API.Controllers
{
    [Route("iGuru/StudentManagement/StudentPromotion")]
    [ApiController]
    public class StudentPromotionController : ControllerBase
    {
        private readonly IStudentPromotionService _studentPromotionService;

        public StudentPromotionController(IStudentPromotionService studentPromotionService)
        {
            _studentPromotionService = studentPromotionService;
        }

        [HttpPost("GetClassPromotion")]
        public async Task<IActionResult> GetClassPromotion([FromBody] GetClassPromotionRequest request)
        {
            var response = await _studentPromotionService.GetClassPromotionAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("UpdateClassPromotionConfiguration")]
        public async Task<IActionResult> UpdateClassPromotionConfiguration([FromBody] UpdateClassPromotionConfigurationRequest request)
        {
            var response = await _studentPromotionService.UpdateClassPromotionConfigurationAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetClassPromotionHistory")]
        public async Task<IActionResult> GetClassPromotionHistory([FromBody] GetClassPromotionHistoryRequest request)
        {
            var response = await _studentPromotionService.GetClassPromotionHistoryAsync(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("GetClassPromotionHistoryExport")]
        public async Task<IActionResult> GetClassPromotionHistoryExport([FromBody] GetClassPromotionHistoryExportRequest request)
        {
            var response = await _studentPromotionService.GetClassPromotionHistoryExportAsync(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            string filePath = response.Data;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Export file not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            if (request.ExportType == 1)
            {
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ClassPromotionHistoryExport.xlsx");
            }
            else if (request.ExportType == 2)
            {
                return File(fileBytes, "text/csv", "ClassPromotionHistoryExport.csv");
            }
            else
            {
                return BadRequest("Invalid ExportType.");
            }
        }

        [HttpPost("PromoteStudents")]
        public async Task<IActionResult> PromoteStudents([FromBody] PromoteStudentsRequest request)
        {
            var response = await _studentPromotionService.PromoteStudentsAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetClasses")]
        public async Task<IActionResult> GetClasses([FromBody] GetClassesRequest request)
        {
            var response = await _studentPromotionService.GetClassesAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetSections")]
        public async Task<IActionResult> GetSections([FromBody] GetSectionsRequest request)
        {
            var response = await _studentPromotionService.GetSectionsAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetStudents")]
        public async Task<IActionResult> GetStudents([FromBody] GetStudentsPromotionRequest request)
        {
            var response = await _studentPromotionService.GetStudentsAsync(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
