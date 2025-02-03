using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Requests.WhatsApp;
using Communication_API.Services.Interfaces.WhatsApp;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Communication_API.Controllers
{
    [Route("iGuru/WhatsApp/[controller]")]
    [ApiController]
    public class WhatsAppController : ControllerBase
    {
        private readonly IWhatsAppService _whatsAppService;

        public WhatsAppController(IWhatsAppService whatsAppService)
        {
            _whatsAppService = whatsAppService;
        }

        [HttpPost("Setup")]
        public async Task<IActionResult> Setup([FromBody] SetupWhatsAppRequest request)
        {
            var response = await _whatsAppService.Setup(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetBalance/{VendorID}")]
        public async Task<IActionResult> GetBalance(int VendorID)
        {
            var response = await _whatsAppService.GetBalance(VendorID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("AddUpdateTemplate")]
        public async Task<IActionResult> AddUpdateTemplate([FromBody] AddUpdateWhatsAppTemplateRequest request)
        {
            var response = await _whatsAppService.AddUpdateTemplate(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetWhatsAppTemplate")]
        public async Task<IActionResult> GetWhatsAppTemplate([FromBody] GetWhatsAppTemplateRequest request)
        {
            var response = await _whatsAppService.GetWhatsAppTemplate(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetWhatsAppTemplateExport")]
        public async Task<IActionResult> GetWhatsAppTemplateExport([FromBody] GetWhatsAppTemplateExportRequest request)
        {
            var response = await _whatsAppService.GetWhatsAppTemplateExport(request);
            if (response.Success)
            {
                // Return file based on ExportType
                var memoryStream = new MemoryStream();
                if (request.ExportType == 1)
                {
                    // Generate Excel
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets.Add("WhatsAppTemplates");
                        worksheet.Cells.LoadFromCollection(response.Data, true);
                        package.Save();
                    }

                    memoryStream.Position = 0;
                    return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "WhatsAppTemplates.xlsx");
                }
                else if (request.ExportType == 2)
                {
                    // Generate CSV
                    var csv = string.Join(",", new[] { "TemplateCode", "TemplateName", "TemplateMessage" }) + "\n";
                    foreach (var template in response.Data)
                    {
                        csv += $"{template.TemplateCode},{template.TemplateName},{template.TemplateMessage}\n";
                    }

                    var csvBytes = System.Text.Encoding.UTF8.GetBytes(csv);
                    memoryStream.Write(csvBytes, 0, csvBytes.Length);
                    memoryStream.Position = 0;
                    return File(memoryStream, "text/csv", "WhatsAppTemplates.csv");
                }
                else
                {
                    return BadRequest("Invalid ExportType");
                }
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPost("Send")]
        public async Task<IActionResult> Send([FromBody] SendWhatsAppRequest request)
        {
            var response = await _whatsAppService.Send(request);
            return StatusCode(response.StatusCode, response);
        }

        //[HttpPost("GetWhatsAppReport")]
        //public async Task<IActionResult> GetWhatsAppReport([FromBody] GetWhatsAppReportRequest request)
        //{
        //    var response = await _whatsAppService.GetWhatsAppReport(request);
        //    return StatusCode(response.StatusCode, response);
        //}

        [HttpPost("SendWhatsAppStudent")]
        public async Task<IActionResult> SendWhatsAppStudent(SendWhatsAppStudentRequest request)
        {
            var response = await _whatsAppService.SendWhatsAppToStudents(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }

        [HttpPost("SendWhatsAppEmployee")]
        public async Task<IActionResult> SendWhatsAppEmployee(SendWhatsAppEmployeeRequest request)
        {
            var response = await _whatsAppService.SendWhatsAppToEmployees(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }

        [HttpPost("UpdateWhatsAppStudentStatus")]
        public async Task<IActionResult> UpdateWhatsAppStudentStatus(UpdateWhatsAppStudentStatusRequest request)
        {
            var response = await _whatsAppService.UpdateWhatsAppStudentStatus(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }

        [HttpPost("UpdateWhatsAppEmployeeStatus")]
        public async Task<IActionResult> UpdateWhatsAppEmployeeStatus(UpdateWhatsAppEmployeeStatusRequest request)
        {
            var response = await _whatsAppService.UpdateWhatsAppEmployeeStatus(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }

        [HttpPost("GetWhatsAppTemplateDDL")]
        public async Task<IActionResult> GetWhatsAppTemplateDDL([FromBody] WhatsAppTemplateDDLRequest request)
        {
            var response = await _whatsAppService.GetWhatsAppTemplateDDL(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("GetWhatsAppStudentReport")]
        public async Task<IActionResult> GetWhatsAppStudentReport([FromBody] GetWhatsAppStudentReportRequest request)
        {
            var response = await _whatsAppService.GetWhatsAppStudentReport(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetWhatsAppStudentReportExport")]
        public async Task<IActionResult> GetWhatsAppStudentReportExport([FromBody] WhatsAppStudentReportExportRequest request)
        {
            // Get the export file content
            var response = await _whatsAppService.GetWhatsAppStudentReportExport(request);

            // Check if the export was successful
            if (response.Success)
            {
                // Get the file path
                string filePath = response.Data;

                // Check if the file exists
                if (System.IO.File.Exists(filePath))
                {
                    // Get the file bytes
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                    // Return the file as a downloadable response
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "WhatsAppReport.xlsx");
                }
                else
                {
                    return BadRequest("File not found.");
                }
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPost("GetWhatsAppEmployeeReport")]
        public async Task<IActionResult> GetWhatsAppEmployeeReport([FromBody] GetWhatsAppEmployeeReportRequest request)
        {
            var response = await _whatsAppService.GetWhatsAppEmployeeReport(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetWhatsAppEmployeeReportExport")]
        public async Task<IActionResult> GetWhatsAppEmployeeReportExport([FromBody] WhatsAppEmployeeReportExportRequest request)
        {
            var response = await _whatsAppService.GetWhatsAppEmployeeReportExport(request);

            if (response.Success)
            {
                string filePath = response.Data;
                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "WhatsAppReport.xlsx");
                }
                else
                {
                    return BadRequest("File not found.");
                }
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPost("GetWhatsAppPlan")]
        public async Task<IActionResult> GetWhatsAppPlan([FromBody] GetWhatsAppPlanRequest request)
        {
            var response = await _whatsAppService.GetWhatsAppPlan(request.WhatsAppVendorID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetWhatsAppTopUpHistory")]
        public async Task<IActionResult> GetWhatsAppTopUpHistory([FromBody] GetWhatsAppTopUpHistoryRequest request)
        {
            var response = await _whatsAppService.GetWhatsAppTopUpHistory(request);

            if (response.Success)
            {
                return Ok(response); // Return 200 OK response
            }

            return BadRequest(response); // Return 400 Bad Request response
        }

        [HttpPost("GetWhatsAppTopUpHistoryExport")]
        public async Task<IActionResult> GetWhatsAppTopUpHistoryExport([FromBody] GetWhatsAppTopUpHistoryExportRequest request)
        {
            var response = await _whatsAppService.GetWhatsAppTopUpHistoryExport(request);

            if (response.Success)
            {
                var memoryStream = new MemoryStream();

                // Handle export type (Excel or CSV)
                if (request.ExportType == 1)
                {
                    // Generate Excel
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets.Add("WhatsAppTopUpHistory");
                        worksheet.Cells.LoadFromCollection(response.Data, true);
                        package.Save();
                    }
                    memoryStream.Position = 0;
                    return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "WhatsAppTopUpHistory.xlsx");
                }
                else if (request.ExportType == 2)
                {
                    // Generate CSV
                    var csv = string.Join(",", new[] { "WhatsAppCredits", "Amount", "TransactionDate" }) + "\n";
                    foreach (var record in response.Data)
                    {
                        csv += $"{record.WhatsAppCredits},{record.Amount},{record.TransactionDate}\n";
                    }

                    var csvBytes = System.Text.Encoding.UTF8.GetBytes(csv);
                    memoryStream.Write(csvBytes, 0, csvBytes.Length);
                    memoryStream.Position = 0;
                    return File(memoryStream, "text/csv", "WhatsAppTopUpHistory.csv");
                }
                else
                {
                    return BadRequest("Invalid ExportType");
                }
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
    }
}
