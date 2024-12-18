using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.ServiceResponse;
using Microsoft.AspNetCore.Mvc;
using Attendance_SE_API.Services.Interfaces;
using System.Threading.Tasks;
using System.Data;
using OfficeOpenXml;
using System.Globalization;

namespace Attendance_SE_API.Controllers
{
    [Route("iGuru/BioMetric/Import")]
    [ApiController]
    public class BioMetricImportController : ControllerBase
    {
        private readonly IBioMetricImportService _bioMetricImportService;

        public BioMetricImportController(IBioMetricImportService bioMetricImportService)
        {
            _bioMetricImportService = bioMetricImportService;
        }

        // Endpoint to download the attendance template
        [HttpPost("DownloadTemplate")]
        public IActionResult DownloadTemplate([FromBody] BioMetricImportRequest request)
        {
            // Ensure start and end dates are provided in the request
            if (string.IsNullOrEmpty(request.StartDate) || string.IsNullOrEmpty(request.EndDate))
            {
                return BadRequest("StartDate and EndDate are required.");
            }

            // Parse the start and end dates
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            // Fetch the attendance data template
            var response = _bioMetricImportService.DownloadBioMetricTemplate(request);

            if (response.Success)
            {
                DataTable attendanceData = response.Data;

                // Generate the Excel file
                byte[] excelFile = _bioMetricImportService.GenerateExcelFile(attendanceData, startDate, endDate);

                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BioMetricAttendanceTemplate.xlsx");
            }

            return BadRequest(response.Message);
        }

        // Endpoint to import biometric attendance data
        [HttpPost("ImportBioMetricAttendance")]
        public async Task<IActionResult> ImportBioMetricAttendance([FromForm] ImportBioMetricAttendanceRequest request, IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            using (var package = new ExcelPackage(excelFile.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming the first worksheet contains the data
                var rowCount = worksheet.Dimension.Rows;
                var colCount = worksheet.Dimension.Columns;

                for (int row = 3; row <= rowCount; row++) // Assuming data starts from row 3
                {
                    var employeeID = worksheet.Cells[row, 1].Text; 
                    var employeeName = worksheet.Cells[row, 2].Text;
                    //var clockIn = worksheet.Cells[row, 6].Text;
                    //var clockOut = worksheet.Cells[row, 7].Text;
                    //var attendanceDate = worksheet.Cells[1, 1].Text; // Assuming date is in the first row of the worksheet

                    // Process each day's attendance data
                    for (int col = 3; col <= colCount; col += 2) // Clock-In and Clock-Out pairs
                    {
                        var attendanceDate = worksheet.Cells[1, col].Text; // The date header
                        var clockIn = worksheet.Cells[row, col].Text;
                        var clockOut = worksheet.Cells[row, col + 1].Text;

                        // Call the service to process and insert data into the database
                        var result = await _bioMetricImportService.ImportBioMetricAttendanceData(
                            request.InstituteID,
                            employeeID,
                            attendanceDate,
                            clockIn,
                            clockOut
                        );

                        if (!result.Success)
                        {
                            return BadRequest(result.Message);
                        }
                    }

                    //// Process and insert the data into the database
                    //var result = await _bioMetricImportService.ImportBioMetricAttendanceData(
                    //    request.InstituteID, 
                    //    employeeID,
                    //    attendanceDate,
                    //    clockIn,
                    //    clockOut
                    //);

                    //if (!result.Success)
                    //{
                    //    return BadRequest(result.Message);
                    //}
                }
            }

            return Ok("BioMetric Attendance data imported successfully.");
        }
    }
}
