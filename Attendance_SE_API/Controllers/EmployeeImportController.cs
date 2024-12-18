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
    [Route("iGuru/Employee/Import")]
    [ApiController]
    public class EmployeeImportController : ControllerBase
    {
        private readonly IEmployeeImportService _employeeImportService;

        public EmployeeImportController(IEmployeeImportService employeeImportService)
        {
            _employeeImportService = employeeImportService;
        }

        [HttpPost("DownloadTemplate")]
        public IActionResult DownloadTemplate([FromBody] EmployeeImportRequest request)
        {
            // Ensure that StartDate and EndDate are provided in the request
            if (string.IsNullOrEmpty(request.StartDate) || string.IsNullOrEmpty(request.EndDate))
            {
                return BadRequest("StartDate and EndDate are required.");
            }

            // Parse the dates from the request
            DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            // Fetch the attendance data using the service
            var response = _employeeImportService.DownloadAttendanceTemplate(request);

            if (response.Success)
            {
                DataTable attendanceData = response.Data;

                // Pass the parsed startDate and endDate to GenerateExcelFile
                byte[] excelFile = _employeeImportService.GenerateExcelFile(attendanceData, startDate, endDate);

                // Get the status data
                var statusData = _employeeImportService.GetStatusData();

                // Add the status sheet to the Excel file
                excelFile = _employeeImportService.AddStatusSheetToExcel(excelFile, statusData);

                // Return the generated Excel file as a response for download
                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeAttendanceTemplate.xlsx");
            }

            return BadRequest(response.Message);
        }


        //// Download the Excel template
        //[HttpPost("DownloadTemplate")]
        //public IActionResult DownloadTemplate([FromBody] ImportEmployeeAttendanceRequest request)
        //{
        //    var response = _employeeImportService.DownloadAttendanceTemplate(request);

        //    if (response.Success)
        //    {
        //        DataTable attendanceData = response.Data;
        //        byte[] excelFile = _employeeImportService.GenerateExcelFile(attendanceData);

        //        var statusData = _employeeImportService.GetStatusData();
        //        excelFile = _employeeImportService.AddStatusSheetToExcel(excelFile, statusData);

        //        return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeAttendanceTemplate.xlsx");
        //    }

        //    return BadRequest(response.Message);
        //}

        // Import employee attendance data from the uploaded Excel file
        [HttpPost("ImportEmployeeAttendance")]
        public async Task<IActionResult> ImportEmployeeAttendance([FromForm] ImportEmployeeAttendanceRequest request, IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            using (var package = new ExcelPackage(excelFile.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming first worksheet
                var worksheet_Status = package.Workbook.Worksheets[1]; // Assuming second worksheet contains status info
                var rowCount = worksheet.Dimension.Rows;
                var colCount = worksheet.Dimension.Columns;

                for (int row = 2; row <= rowCount; row++) // Loop through employee rows (starting from row 2)
                {
                    var employeeID = worksheet.Cells[row, 1].Text; // Column A for EmployeeID
                    var departmentID = worksheet.Cells[row, 2].Text; // Column B for DepartmentID
                    var designationID = worksheet.Cells[row, 3].Text; // Column C for DesignationID
                    //var employeeCode = worksheet.Cells[row, 4].Text; // Column D for EmployeeCode

                    // Process Attendance for each day (starting from column E)
                    for (int col = 7; col <= colCount; col++) // Columns E onwards for attendance dates
                    {
                        var attendanceDate = worksheet.Cells[1, col].Text; // Row 1 for AttendanceDate
                        var shortName = worksheet.Cells[row, col].Text; // ShortName for status from dynamic column

                        var statusID = GetStatusIDByShortName(worksheet_Status, shortName);

                        if (string.IsNullOrEmpty(statusID))
                        {
                            return BadRequest($"Invalid status for ShortName '{shortName}' on {attendanceDate}");
                        }

                        // Call the service to insert data
                        var result = await _employeeImportService.ImportAttendanceData(
                            request.InstituteID,
                            departmentID,
                            designationID,
                            attendanceDate,
                            employeeID,
                            statusID
                        );

                        if (!result.Success)
                        {
                            return BadRequest(result.Message); // Return an error if insertion fails
                        }
                    }
                }

                return Ok("Employee Attendance data imported successfully.");
            }
        }

        // Helper method to get StatusID based on ShortName
        private string GetStatusIDByShortName(ExcelWorksheet worksheet, string shortName)
        {
            var rowCount = worksheet.Dimension.Rows;
            var statusSheetStartRow = 2; // Assuming row 1 contains headers

            for (int row = statusSheetStartRow; row <= rowCount; row++)
            {
                var statusShortName = worksheet.Cells[row, 3].Text; // Assuming ShortName is in column C

                if (statusShortName.Equals(shortName, StringComparison.OrdinalIgnoreCase))
                {
                    return worksheet.Cells[row, 1].Text; // Assuming StatusID is in column A
                }
            }

            return null; // Return null if ShortName is not found
        }
    }
}
