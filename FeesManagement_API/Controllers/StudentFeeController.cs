using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Services.Interfaces;
using OfficeOpenXml;
using System.Text;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/FeeAssignments/IndividualStudentFees")]
    [ApiController]
    public class StudentFeeController : ControllerBase
    {
        private readonly IStudentFeeService _studentFeeService;

        public StudentFeeController(IStudentFeeService studentFeeService)
        {
            _studentFeeService = studentFeeService;
        }

        [HttpPost("GetStudentFees")]
        public IActionResult GetStudentFees([FromBody] StudentFeeRequest request)
        {
            var response = _studentFeeService.GetStudentFees(request);
            return Ok(response);
        }

        [HttpPost("DiscountStudentFees")]
        public IActionResult DiscountStudentFees([FromBody] DiscountStudentFeesRequest request)
        {
            var response = _studentFeeService.DiscountStudentFees(request);
            return Ok(response);
        }

        [HttpPost("GetFeesChangeLogs")]
        public IActionResult GetFeesChangeLogs([FromBody] GetFeesChangeLogsRequest request)
        {
            var response = _studentFeeService.GetFeesChangeLogs(request);
            return Ok(response);
        }
          
        [HttpPost("GetStudentFeesExport")]
        public async Task<IActionResult> GetStudentFeesExport([FromBody] GetStudentFeesExportRequest request)
        {
            try
            {
                var fileBytes = await _studentFeeService.GetStudentFeesExportAsync(request);
                if (fileBytes == null || fileBytes.Length == 0)
                    return NotFound("No records found.");

                string fileName = request.ExportType == 1 ? "StudentFees.xlsx" : "StudentFees.csv";
                string contentType = request.ExportType == 1
                    ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    : "text/csv";

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("GetFeesChangeLogsExport")]
        public async Task<IActionResult> GetFeesChangeLogsExport([FromBody] GetFeesChangeLogsExportRequest request)
        {
            try
            {
                var fileBytes = await _studentFeeService.GetFeesChangeLogsExportAsync(request);
                if (fileBytes == null || fileBytes.Length == 0)
                    return NotFound("No records found.");

                string fileName = request.ExportType == 1 ? "FeesChangeLogs.xlsx" : "FeesChangeLogs.csv";
                string contentType = request.ExportType == 1
                    ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    : "text/csv";

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
