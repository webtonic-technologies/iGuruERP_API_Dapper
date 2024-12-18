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
    [Route("iGuru/Student/Import")]
    [ApiController]
    public class StudentImportController : ControllerBase
    {
        private readonly IStudentImportService _studentImportService;

        public StudentImportController(IStudentImportService studentImportService)
        {
            _studentImportService = studentImportService;
        }



        [HttpPost("DownloadTemplate")]
        public IActionResult DownloadTemplate([FromBody] StudentImportRequest request)
        {
            // Fetch the attendance data directly as a DataTable
            var response = _studentImportService.DownloadAttendanceTemplate(request);

            if (response.Success)
            {
                // Get the attendance data from the service response
                DataTable attendanceData = response.Data;

                // Parse StartDate and EndDate using DateTime.ParseExact to handle "DD-MM-YYYY" format
                DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                // Call the service method to generate the Excel file with dynamic columns
                byte[] excelFile = _studentImportService.GenerateExcelFile(attendanceData, startDate, endDate);

                // Fetch the status data for Sheet 2
                var statusData = _studentImportService.GetStatusData();

                // Add the status sheet to the Excel file
                excelFile = _studentImportService.AddStatusSheetToExcel(excelFile, statusData);


                // Return the generated Excel file as a downloadable file
                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentAttendanceTemplate.xlsx");
            }

            return BadRequest(response.Message);
        }

        [HttpPost("ImportStudentAttendance")]
        public async Task<IActionResult> ImportStudentAttendance([FromForm] ImportStudentAttendanceRequest request, IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Reading the Excel file using EPPlus library
            using (var package = new ExcelPackage(excelFile.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming first worksheet
                var worksheet_Status = package.Workbook.Worksheets[1]; // Assuming first worksheet

                var rowCount = worksheet.Dimension.Rows;
                var colCount = worksheet.Dimension.Columns;

                // Loop through rows and process data
                for (int row = 2; row <= rowCount; row++) // Assuming row 1 is header
                {
                    var studentID = worksheet.Cells[row, 1].Text; // Column A for StudentID
                    var classID = worksheet.Cells[row, 2].Text; // Column B for ClassID
                    var sectionID = worksheet.Cells[row, 3].Text; // Column C for SectionID

                    // Process Attendance for each day (starting from column H onward)
                    for (int col = 8; col <= colCount; col++) // Columns H onwards
                    {
                        var attendanceDate = worksheet.Cells[1, col].Text; // Row 1 for AttendanceDate
                                                                           
                        //var statusID = worksheet.Cells[row, col].Text; // Status from dynamic column
                        var statusID = GetStatusIDByShortName(worksheet_Status, worksheet.Cells[row, col].Text.ToString());


                        // Call the service to insert data
                        var result = await _studentImportService.ImportAttendanceData(
                            request.InstituteID,
                            classID,
                            sectionID,
                            attendanceDate,
                            studentID,
                            statusID,
                            request.AcademicYearCode
                        );

                        if (!result.Success)
                        {
                            return BadRequest(result.Message); // Return an error if insertion fails
                        }
                    }
                }

                return Ok("Student Attendance data imported successfully.");
            }
        }

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
