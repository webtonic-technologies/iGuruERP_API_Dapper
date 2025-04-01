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



        [HttpPost("IndividualStudentFees/GetStudentFeesExport")]
        public async Task<IActionResult> GetStudentFeesExport([FromBody] GetStudentFeesExportRequest request)
        {
            var response = await _studentFeeService.GetStudentFeesExport(request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            var studentFees = response.Data; // List<StudentFeeResponse>
            var memoryStream = new MemoryStream();

            if (request.ExportType == 1)
            {
                // ========== Export as Excel ==========
                using (var package = new ExcelPackage(memoryStream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("StudentFees");

                    // Example headers (adjust as needed)
                    worksheet.Cells[1, 1].Value = "AdmissionNo";
                    worksheet.Cells[1, 2].Value = "StudentName";
                    worksheet.Cells[1, 3].Value = "RollNo";
                    worksheet.Cells[1, 4].Value = "ClassName";
                    worksheet.Cells[1, 5].Value = "SectionName";
                    worksheet.Cells[1, 6].Value = "ConcessionGroup";
                    worksheet.Cells[1, 7].Value = "TotalFeeAmount";
                    worksheet.Cells[1, 8].Value = "TotalLateFee";

                    // If you also want to show details pivoted or in separate columns, you'd have to add more columns 
                    // or possibly create additional rows. 
                    // For simplicity, let's just show the top-level fields.

                    int row = 2;
                    foreach (var fee in studentFees)
                    {
                        worksheet.Cells[row, 1].Value = fee.AdmissionNo;
                        worksheet.Cells[row, 2].Value = fee.StudentName;
                        worksheet.Cells[row, 3].Value = fee.RollNo;
                        worksheet.Cells[row, 4].Value = fee.ClassName;
                        worksheet.Cells[row, 5].Value = fee.SectionName;
                        worksheet.Cells[row, 6].Value = fee.ConcessionGroup;
                        worksheet.Cells[row, 7].Value = fee.TotalFeeAmount;
                        worksheet.Cells[row, 8].Value = fee.TotalLateFee;
                        row++;
                    }

                    package.Save();
                }

                memoryStream.Position = 0;
                return File(
                    memoryStream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "StudentFees.xlsx"
                );
            }
            else if (request.ExportType == 2)
            {
                // ========== Export as CSV ==========
                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("AdmissionNo,StudentName,RollNo,ClassName,SectionName,ConcessionGroup,TotalFeeAmount,TotalLateFee");

                foreach (var fee in studentFees)
                {
                    // Be careful about commas in data (escape if needed). 
                    csvBuilder.AppendLine(
                        $"{fee.AdmissionNo}," +
                        $"{fee.StudentName}," +
                        $"{fee.RollNo}," +
                        $"{fee.ClassName}," +
                        $"{fee.SectionName}," +
                        $"{(fee.ConcessionGroup ?? "").Replace(",", " ")}," +
                        $"{fee.TotalFeeAmount}," +
                        $"{fee.TotalLateFee}"
                    );
                }

                var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
                memoryStream.Write(csvBytes, 0, csvBytes.Length);
                memoryStream.Position = 0;

                return File(memoryStream, "text/csv", "StudentFees.csv");
            }
            else
            {
                return BadRequest("Invalid ExportType");
            }
        }
    }
}
