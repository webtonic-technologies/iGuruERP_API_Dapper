using Lesson_API.DTOs.Requests;
using Lesson_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lesson_API.Controllers
{
    [Route("iGuru/Assignment/AssignmentReports")]
    [ApiController]
    public class AssignmentReportController : ControllerBase
    {
        private readonly IAssignmentReportService _assignmentReportService;

        public AssignmentReportController(IAssignmentReportService assignmentReportService)
        {
            _assignmentReportService = assignmentReportService;
        }

        [HttpPost("GetAssignmentsReports")]
        public async Task<IActionResult> GetAssignmentsReports([FromBody] GetAssignmentsReportsRequest request)
        {
            var response = await _assignmentReportService.GetAssignmentsReports(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetAssignmentsReportExport")]
        public async Task<IActionResult> GetAssignmentsReportExport([FromBody] GetAssignmentsReportsExportRequest request)
        {
            var response = await _assignmentReportService.GetAssignmentsReportExport(request);

            if (response.Success)
            {
                return File(response.Data,
                            response.StatusCode == 200 ?
                            (request.ExportType == 1 ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv") :
                            "application/json",
                            "AssignmentsReportExport" + (request.ExportType == 1 ? ".xlsx" : ".csv"));
            }

            return BadRequest(response);
        }
    }
}
