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
    [Route("iGuru/GeoFencing/Import")]
    [ApiController]
    public class GeoFencingImportController : ControllerBase
    {
        private readonly IGeoFencingImportService _geoFencingImportService;

        public GeoFencingImportController(IGeoFencingImportService geoFencingImportService)
        {
            _geoFencingImportService = geoFencingImportService;
        }

        [HttpPost("DownloadTemplate")]
        public IActionResult DownloadTemplate([FromBody] GeoFencingImportRequest request)
        {
            if (string.IsNullOrEmpty(request.StartDate) || string.IsNullOrEmpty(request.EndDate))
            {
                return BadRequest("StartDate and EndDate are required.");
            }

            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var response = _geoFencingImportService.DownloadGeoFencingTemplate(request);

            if (response.Success)
            {
                DataTable geoFencingData = response.Data;

                byte[] excelFile = _geoFencingImportService.GenerateExcelFile(geoFencingData, startDate, endDate);

                var statusData = _geoFencingImportService.GetStatusData();
                excelFile = _geoFencingImportService.AddStatusSheetToExcel(excelFile, statusData);

                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GeoFencingAttendanceTemplate.xlsx");
            }

            return BadRequest(response.Message);
        }

        [HttpPost("ImportGeoFencingAttendance")]
        public async Task<IActionResult> ImportGeoFencingAttendance([FromForm] ImportGeoFencingAttendanceRequest request, IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Read the uploaded Excel file using EPPlus
            using (var package = new ExcelPackage(excelFile.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming the first worksheet contains the data
                var rowCount = worksheet.Dimension.Rows;
                var colCount = worksheet.Dimension.Columns;

                // Process each row from the Excel file
                for (int row = 3; row <= rowCount; row++) // Starting from row 2, assuming row 1 has headers
                {
                    var employeeID = worksheet.Cells[row, 1].Text;
                    var departmentID = worksheet.Cells[row, 2].Text;
                    var designationID = worksheet.Cells[row, 3].Text;
                    var employeeCode = worksheet.Cells[row, 4].Text;
                    var employeeName = worksheet.Cells[row, 5].Text;
                    var mobileNumber = worksheet.Cells[row, 6].Text;

                    // Process each day's attendance data
                    for (int col = 7; col <= colCount; col += 2) // Clock-In and Clock-Out pairs
                    {
                        var date = worksheet.Cells[1, col].Text; // The date header
                        var clockIn = worksheet.Cells[row, col].Text;
                        var clockOut = worksheet.Cells[row, col + 1].Text;

                        // Call the service to process and insert data into the database
                        var result = await _geoFencingImportService.ImportAttendanceData(
                            request.InstituteID,
                            departmentID, 
                            date,
                            employeeID,
                            clockIn,
                            clockOut
                        );

                        if (!result.Success)
                        {
                            return BadRequest(result.Message);
                        }
                    }
                }
            }

            return Ok("GeoFencing Attendance data imported successfully.");
        }
    }
}
