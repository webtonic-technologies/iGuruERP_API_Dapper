using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Attendance_SE_API.ServiceResponse;
using Attendance_SE_API.DTOs.Responses;

namespace Attendance_SE_API.Controllers
{
    [Route("iGuru/Employee/EmployeeAttendanceReport")]
    [ApiController]
    public class EmployeeAttendanceReportController : ControllerBase
    {
        private readonly IEmployeeAttendanceReportService _employeeAttendanceReportService;

        public EmployeeAttendanceReportController(IEmployeeAttendanceReportService employeeAttendanceReportService)
        {
            _employeeAttendanceReportService = employeeAttendanceReportService;
        }

        //[HttpPost("GetAttendanceReport")]
        //public async Task<IActionResult> GetAttendanceReport([FromBody] EmployeeAttendanceReportRequest request)
        //{
        //    if (request == null)
        //    {
        //        return BadRequest("Request body cannot be null.");
        //    }

        //    // Validate request properties if necessary
        //    if (string.IsNullOrEmpty(request.StartDate) || string.IsNullOrEmpty(request.EndDate))
        //    {
        //        return BadRequest("StartDate and EndDate are required.");
        //    }

        //    var response = await _employeeAttendanceReportService.GetAttendanceReport(request);
        //    return StatusCode(response.StatusCode, response);
        //}

        [HttpPost("GetAttendanceReport")]
        public async Task<IActionResult> GetAttendanceReport([FromBody] EmployeeAttendanceReportRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            // Validate mandatory fields
            if (string.IsNullOrEmpty(request.StartDate) || string.IsNullOrEmpty(request.EndDate))
            {
                return BadRequest("StartDate and EndDate are required.");
            }

            // Validate pagination parameters
            if (request.PageNumber <= 0 || request.PageSize <= 0)
            {
                return BadRequest("PageNumber and PageSize must be greater than zero.");
            }

            var response = await _employeeAttendanceReportService.GetAttendanceReport(request);
            return StatusCode(response.StatusCode, response);
        }


        //[HttpPost("GetAttendanceGeoFencingReport")]
        //public async Task<ActionResult<ServiceResponse<GetAttendanceGeoFencingReportResponse>>> GetAttendanceGeoFencingReport([FromBody] GetAttendanceGeoFencingReportRequest request)
        //{
        //    var response = await _employeeAttendanceReportService.GetAttendanceGeoFencingReport(request);
        //    return Ok(response);
        //}

        [HttpPost("GetAttendanceGeoFencingReport")]
        public async Task<ActionResult<ServiceResponse<GetAttendanceGeoFencingReportResponse>>> GetAttendanceGeoFencingReport([FromBody] GetAttendanceGeoFencingReportRequest request)
        {
            var response = await _employeeAttendanceReportService.GetAttendanceGeoFencingReport(request);
            return Ok(response);
        }


        [HttpPost("GetAttendanceGeoFencingReportExportExcel")]
        public async Task<IActionResult> GetAttendanceGeoFencingReportExportExcel([FromBody] GetAttendanceGeoFencingReportRequest request)
        {
            // Fetch the Excel report data
            var stream = await _employeeAttendanceReportService.GenerateExcelReport(request);

            // Check if the stream is null
            if (stream == null)
            {
                return BadRequest("Failed to generate the report.");
            }

            // Return the Excel file as a response
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AttendanceReport.xlsx");
        }

        [HttpPost("GetAttendanceGeoFencingReportExportCSV")]
        public async Task<IActionResult> GetAttendanceGeoFencingReportExportCSV([FromBody] GetAttendanceGeoFencingReportRequest request)
        {
            // Generate the CSV report
            var stream = await _employeeAttendanceReportService.GenerateCSVReport(request);

            // Check if the stream is null
            if (stream == null)
            {
                return BadRequest("Failed to generate the report.");
            }

            // Return the CSV file as a response
            return File(stream, "text/csv", "AttendanceReport.csv");
        }


        [HttpPost("GetAttendanceMode")]
        public async Task<IActionResult> GetAttendanceMode()
        {
            var attendanceModes = await _employeeAttendanceReportService.GetAttendanceMode();
            return Ok(new { Success = true, Message = "Attendance modes fetched successfully.", Data = attendanceModes });
        }

        [HttpPost("GetAttendanceBioMericReport")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<GetAttendanceBioMericReportResponse>>>> GetAttendanceBioMericReport([FromBody] GetAttendanceBioMericReportRequest request)
        {
            var response = await _employeeAttendanceReportService.GetAttendanceBioMericReport(request);
            return Ok(response);
        }

        [HttpPost("GetAttendanceBioMetricReportExportExcel")]
        public async Task<IActionResult> GetAttendanceBioMetricReportExportExcel([FromBody] GetAttendanceBioMericReportRequest request)
        {
            // Fetch the Excel report data
            var stream = await _employeeAttendanceReportService.GenerateBioMetricExcelReport(request);

            // Check if the stream is null
            if (stream == null)
            {
                return BadRequest("Failed to generate the report.");
            }

            // Return the Excel file as a response
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BioMericAttendanceReport.xlsx");
        }


        [HttpPost("GetAttendanceBioMetricReportExportCSV")]
        public async Task<IActionResult> GetAttendanceBioMetricReportExportCSV([FromBody] GetAttendanceBioMericReportRequest request)
        {
            // Generate the CSV report
            var stream = await _employeeAttendanceReportService.GenerateBioMetricCSVReport(request);

            // Check if the stream is null
            if (stream == null)
            {
                return BadRequest("Failed to generate the report.");
            }

            // Return the CSV file as a response
            return File(stream, "text/csv", "BioMericAttendanceReport.csv");
        }
    }
}
