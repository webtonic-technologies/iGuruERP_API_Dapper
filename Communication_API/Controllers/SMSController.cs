using Communication_API.DTOs.Requests;
using Communication_API.DTOs.Requests.SMS;
using Communication_API.Services.Interfaces.SMS;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Communication_API.Controllers
{
    [Route("iGuru/SMS/[controller]")]
    [ApiController]
    public class SMSController : ControllerBase
    {
        private readonly ISMSService _smsService;

        public SMSController(ISMSService smsService)
        {
            _smsService = smsService;
        }

        [HttpPost("Setup")]
        public async Task<IActionResult> Setup([FromBody] SetupSMSConfigurationRequest request)
        {
            var response = await _smsService.SetupSMSConfiguration(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetSMSBalance/{VendorID}")]
        public async Task<IActionResult> GetSMSBalance(int VendorID)
        {
            var response = await _smsService.GetSMSBalance(VendorID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("CreateSMSTemplate")]
        public async Task<IActionResult> CreateSMSTemplate([FromBody] CreateSMSTemplateRequest request)
        {
            var response = await _smsService.CreateSMSTemplate(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllSMSTemplate")]
        public async Task<IActionResult> GetAllSMSTemplate([FromBody] GetAllSMSTemplateRequest request)
        {
            var response = await _smsService.GetAllSMSTemplate(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllSMSTemplateExport")]
        public async Task<IActionResult> GetAllSMSTemplateExport([FromBody] SMSTemplateExportRequest request)
        {
            var response = await _smsService.GetAllSMSTemplateExport(request);
            if (response.Success)
            {
                // Return file based on ExportType
                var memoryStream = new MemoryStream();
                if (request.ExportType == 1)
                {
                    // Generate Excel
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets.Add("SMSTemplates");
                        worksheet.Cells.LoadFromCollection(response.Data, true);
                        package.Save();
                    }

                    memoryStream.Position = 0;
                    return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SMSTemplates.xlsx");
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
                    return File(memoryStream, "text/csv", "SMSTemplates.csv");
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

        [HttpPost("SendNewSMS")]
        public async Task<IActionResult> SendNewSMS([FromBody] SendNewSMSRequest request)
        {
            var response = await _smsService.SendNewSMS(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetSMSStudentReport")]
        public async Task<IActionResult> GetSMSStudentReport([FromBody] GetSMSStudentReportRequest request)
        {
            var response = await _smsService.GetSMSStudentReport(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetSMSStudentReportExport")]
        public async Task<IActionResult> GetSMSStudentReportExport([FromBody] SMSStudentReportExportRequest request)
        {
            // Get the export file content
            var response = await _smsService.GetSMSStudentReportExport(request);

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
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SMSReport.xlsx");
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

        [HttpPost("GetSMSEmployeeReport")]
        public async Task<IActionResult> GetSMSEmployeeReport([FromBody] GetSMSEmployeeReportRequest request)
        {
            var response = await _smsService.GetSMSEmployeeReport(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetSMSEmployeeReportExport")]
        public async Task<IActionResult> GetSMSEmployeeReportExport([FromBody] SMSEmployeeReportExportRequest request)
        {
            var response = await _smsService.GetSMSEmployeeReportExport(request);

            if (response.Success)
            {
                string filePath = response.Data;
                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SMSReport.xlsx");
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



        [HttpPost("SendSMSStudent")]
        public async Task<IActionResult> SendSMSStudent(SendSMSStudentRequest request)
        {
            // Call the service layer to send SMS
            var response = await _smsService.SendSMSStudent(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }

        [HttpPost("SendSMSEmployee")]
        public async Task<IActionResult> SendSMSEmployee(SendSMSEmployeeRequest request)
        {
            // Call the service layer to send SMS to employees
            var response = await _smsService.SendSMSEmployee(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }

        [HttpPost("UpdateSMSStudentStatus")]
        public async Task<IActionResult> UpdateSMSStudentStatus(UpdateSMSStudentStatusRequest request)
        {
            // Call the service layer to update SMS status for the student
            var response = await _smsService.UpdateSMSStudentStatus(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }

        [HttpPost("UpdateSMSEmployeeStatus")]
        public async Task<IActionResult> UpdateSMSEmployeeStatus(UpdateSMSEmployeeStatusRequest request)
        {
            // Call the service layer to update the SMS status for the employee
            var response = await _smsService.UpdateSMSEmployeeStatus(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }

        [HttpPost("GetSMSTemplateDDL")]
        public async Task<IActionResult> GetSMSTemplateDDL([FromBody] SMSTemplateDDLRequest request)
        {
            var response = await _smsService.GetSMSTemplateDDL(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetSMSPlan")]
        public async Task<IActionResult> GetSMSPlan([FromBody] SMSVendorRequest request)
        {
            // Pass the SMSVendorID to the service method
            var response = await _smsService.GetSMSPlan(request.SMSVendorID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetSMSTopUpHistory")]
        public async Task<IActionResult> GetSMSTopUpHistory([FromBody] GetSMSTopUpHistoryRequest request)
        {
            var response = await _smsService.GetSMSTopUpHistory(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetSMSTopUpHistoryExport")]
        public async Task<IActionResult> GetSMSTopUpHistoryExport([FromBody] GetSMSTopUpHistoryExportRequest request)
        {
            var response = await _smsService.GetSMSTopUpHistoryExport(request);

            if (response.Success)
            {
                var memoryStream = new MemoryStream();

                if (request.ExportType == 1)
                {
                    // Generate Excel
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets.Add("SMSTopUpHistory");
                        worksheet.Cells.LoadFromCollection(response.Data, true);
                        package.Save();
                    }

                    memoryStream.Position = 0;
                    return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SMSTopUpHistory.xlsx");
                }
                else if (request.ExportType == 2)
                {
                    // Generate CSV
                    var csv = string.Join(",", new[] { "SMSCredits", "Amount", "TransactionDate" }) + "\n";
                    foreach (var record in response.Data)
                    {
                        csv += $"{record.SMSCredits},{record.Amount},{record.TransactionDate}\n";
                    }

                    var csvBytes = System.Text.Encoding.UTF8.GetBytes(csv);
                    memoryStream.Write(csvBytes, 0, csvBytes.Length);
                    memoryStream.Position = 0;
                    return File(memoryStream, "text/csv", "SMSTopUpHistory.csv");
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
