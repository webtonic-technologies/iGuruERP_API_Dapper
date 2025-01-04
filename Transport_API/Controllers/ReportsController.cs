using Microsoft.AspNetCore.Mvc;
using Transport_API.DTOs.Requests;
using Transport_API.Services.Interfaces;

namespace Transport_API.Controllers
{
    [Route("iGuru/Reports/Reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsService _reportsService;

        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        //[HttpPost("GetTransportPendingFeeReport")]
        //public async Task<IActionResult> GetTransportPendingFeeReport([FromBody] GetReportsRequest request)
        //{
        //    try
        //    {
        //        var response = await _reportsService.GetTransportPendingFeeReport(request);
        //        return StatusCode(response.StatusCode, response);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        [HttpPost("GetEmployeeTransportationReport")]
        public async Task<IActionResult> GetEmployeeTransportationReport(GetReportsRequest request)
        {
            try
            {
                var response = await _reportsService.GetEmployeeTransportationReport(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GetStudentTransportReport")]
        public async Task<IActionResult> GetStudentTransportReport([FromBody] GetReportsRequest request)
        {
            try
            {
                var response = await _reportsService.GetStudentTransportReport(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("GetTransportAttendanceReport")]
        public async Task<IActionResult> GetTransportAttendanceReport([FromBody] TransportAttendanceReportRequest request)
        {
            try
            {
                var response = await _reportsService.GetTransportAttendanceReport(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
         

        [HttpPost("GetStudentsReport")]
        public async Task<IActionResult> GetStudentsReport(StudentsReportRequest request)
        {
            try
            {
                var response = await _reportsService.GetStudentsReport(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetTransportationPendingFeeReport")]
        public async Task<IActionResult> GetTransportationPendingFeeReport([FromBody] GetTransportationPendingFeeReportRequest request)
        {
            if (request == null || request.InstituteID == 0 || request.RoutePlanID == 0)
                return BadRequest("Invalid request data.");

            var response = await _reportsService.GetTransportationPendingFeeReport(request.InstituteID, request.RoutePlanID);

            if (response.Data == null || !response.Data.Any())
                return NotFound("No data found.");

            return Ok(response);
        }

        [HttpPost("GetDeAllocationReport")]
        public async Task<IActionResult> GetDeAllocationReport([FromBody] GetDeAllocationReportRequest request)
        {
            var result = await _reportsService.GetDeAllocationReport(request);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        //Excel Export

        [HttpPost("GetEmployeeTransportationReportExportExcel")]
        public async Task<IActionResult> GetEmployeeTransportationReportExportExcel(GetReportsRequest request)
        {
            var excelData = await _reportsService.GetEmployeeTransportationReportExportExcel(request);

            if (excelData != null)
            {
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeTransportationReport.xlsx");
            }

            return NoContent();
        }

        [HttpPost("GetStudentTransportReportExportExcel")]
        public async Task<IActionResult> GetStudentTransportReportExportExcel(GetReportsRequest request)
        {
            var excelData = await _reportsService.GetStudentTransportReportExportExcel(request);

            if (excelData != null)
            {
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentTransportReport.xlsx");
            }

            return NoContent();
        }

        [HttpPost("GetTransportAttendanceReportExportExcel")]
        public async Task<IActionResult> GetTransportAttendanceReportExportExcel([FromBody] TransportAttendanceReportRequest request)
        {
            var excelData = await _reportsService.GetTransportAttendanceReportExportExcel(request);

            if (excelData == null)
            {
                return NoContent(); // Return 204 if no data
            }

            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TransportAttendanceReport.xlsx");
        }



        [HttpPost("GetStudentsReportExportExcel")]
        public async Task<IActionResult> GetStudentsReportExportExcel(StudentsReportRequest request)
        {
            var excelData = await _reportsService.GetStudentsReportExportExcel(request);

            if (excelData != null)
            {
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentsReport.xlsx");
            }

            return NoContent();
        }


        [HttpPost("GetTransportationPendingFeeReportExportExcel")]
        public async Task<IActionResult> GetTransportationPendingFeeReportExportExcel([FromBody] TransportationFeeReportExExcelRequest request)
        {
            try
            {
                var excelData = await _reportsService.GetTransportationPendingFeeReportExportExcel(request);

                if (excelData != null)
                {
                    return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TransportationPendingFeeReport.xlsx");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDeAllocationReportExportExcel")]
        public async Task<IActionResult> GetDeAllocationReportExportExcel([FromBody] GetDeAllocationReportExportExcelRequest request)
        {
            var excelData = await _reportsService.GetDeAllocationReportExportExcel(request);

            if (excelData != null)
            {
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DeAllocationReport.xlsx");
            }

            return NoContent();
        }

        // CSV Export Services

        [HttpPost("GetEmployeeTransportationReportExportCSV")]
        public async Task<IActionResult> GetEmployeeTransportationReportExportCSV([FromBody] GetReportsRequest request)
        {
            var csvData = await _reportsService.GetEmployeeTransportationReportExportCSV(request);

            if (csvData == null)
            {
                return NoContent(); // Return 204 if no data
            }

            return File(csvData, "text/csv", "EmployeeTransportationReport.csv");
        }

        [HttpPost("GetStudentTransportReportExportCSV")]
        public async Task<IActionResult> GetStudentTransportReportExportCSV([FromBody] GetReportsRequest request)
        {
            var csvData = await _reportsService.GetStudentTransportReportExportCSV(request);

            if (csvData == null)
            {
                return NoContent(); // Return 204 if no data
            }

            return File(csvData, "text/csv", "StudentTransportReport.csv");
        }

        [HttpPost("GetTransportAttendanceReportExportCSV")]
        public async Task<IActionResult> GetTransportAttendanceReportExportCSV([FromBody] TransportAttendanceReportRequest request)
        {
            var csvData = await _reportsService.GetTransportAttendanceReportExportCSV(request);

            if (csvData == null)
            {
                return NoContent(); // Return 204 if no data
            }

            return File(csvData, "text/csv", "TransportAttendanceReport.csv");
        }

        [HttpPost("GetStudentsReportExportCSV")]
        public async Task<IActionResult> GetStudentsReportExportCSV([FromBody] StudentsReportRequest request)
        {
            var csvData = await _reportsService.GetStudentsReportExportCSV(request);

            if (csvData == null)
            {
                return NoContent(); // Return 204 if no data
            }

            return File(csvData, "text/csv", "StudentsReport.csv");
        }

        [HttpPost("GetTransportationPendingFeeReportExportCSV")]
        public async Task<IActionResult> GetTransportationPendingFeeReportExportCSV([FromBody] TransportationFeeReportExExcelRequest request)
        {
            try
            {
                var csvData = await _reportsService.GetTransportationPendingFeeReportExportCSV(request);

                if (csvData == null)
                {
                    return NoContent();
                }

                return File(csvData, "text/csv", "TransportationPendingFeeReport.csv");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDeAllocationReportExportCSV")]
        public async Task<IActionResult> GetDeAllocationReportExportCSV([FromBody] GetDeAllocationReportExportExcelRequest request)
        {
            try
            {
                var csvData = await _reportsService.GetDeAllocationReportExportCSV(request);

                if (csvData != null)
                {
                    return File(csvData, "text/csv", "DeAllocationReport.csv");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
