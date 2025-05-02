using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.Services.Interfaces;
using OfficeOpenXml;
using System.Text;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/FeeAssignments/StudentPreviousFees")]
    [ApiController]
    public class StudentPreviousFeeController : ControllerBase
    {
        private readonly IStudentPreviousFeeService _studentPreviousFeeService;

        public StudentPreviousFeeController(IStudentPreviousFeeService studentPreviousFeeService)
        {
            _studentPreviousFeeService = studentPreviousFeeService;
        }

        [HttpPost("GetStudentPreviousFees")]
        public IActionResult GetStudentPreviousFees([FromBody] StudentPreviousFeeRequest request)
        {
            var response = _studentPreviousFeeService.GetStudentPreviousFees(request);
            return Ok(response);
        } 

        //[HttpPost("DiscountStudentPreviousFees")]
        //public IActionResult DiscountStudentPreviousFees([FromBody] List<DiscountStudentPreviousFeesRequest> requests)
        //{
        //    if (requests == null || requests.Count == 0)
        //        return BadRequest("No discount records provided.");

        //    var serviceResponse = _studentPreviousFeeService.DiscountStudentPreviousFees(requests);
        //    return StatusCode(serviceResponse.StatusCode, serviceResponse);
        //}


        //[HttpPost("GetPreviousFeesChangeLogs")]
        //public IActionResult GetPreviousFeesChangeLogs([FromBody] GetPreviousFeesChangeLogsRequest request)
        //{
        //    var response = _studentPreviousFeeService.GetPreviousFeesChangeLogs(request);
        //    return Ok(response);
        //}
          
        [HttpPost("GetStudentPreviousFeesExport")]
        public async Task<IActionResult> GetStudentPreviousFeesExport([FromBody] GetStudentPreviousFeesExportRequest request)
        {
            try
            {
                var fileBytes = await _studentPreviousFeeService.GetStudentPreviousFeesExportAsync(request);
                if (fileBytes == null || fileBytes.Length == 0)
                    return NotFound("No records found.");

                string fileName = request.ExportType == 1 ? "StudentPreviousFees.xlsx" : "StudentPreviousFees.csv";
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

        //[HttpPost("GetPreviousFeesChangeLogsExport")]
        //public async Task<IActionResult> GetPreviousFeesChangeLogsExport([FromBody] GetPreviousFeesChangeLogsExportRequest request)
        //{
        //    try
        //    {
        //        var fileBytes = await _studentPreviousFeeService.GetPreviousFeesChangeLogsExportAsync(request);
        //        if (fileBytes == null || fileBytes.Length == 0)
        //            return NotFound("No records found.");

        //        string fileName = request.ExportType == 1 ? "PreviousFeesChangeLogs.xlsx" : "PreviousFeesChangeLogs.csv";
        //        string contentType = request.ExportType == 1
        //            ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        //            : "text/csv";

        //        return File(fileBytes, contentType, fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}
    }
}
