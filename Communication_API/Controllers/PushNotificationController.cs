using Communication_API.DTOs.Requests.NoticeBoard;
using Communication_API.DTOs.Requests.PushNotification;
using Communication_API.DTOs.Requests.SMS;
using Communication_API.Services.Interfaces.PushNotification;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Communication_API.Controllers
{
    [Route("iGuru/PushNotification/[controller]")]
    [ApiController]
    public class PushNotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public PushNotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("PushNotification")]
        public async Task<IActionResult> PushNotification([FromBody] TriggerNotificationRequest request)
        {
            var response = await _notificationService.TriggerNotification(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetPushNotificationStudent")]
        public async Task<IActionResult> GetPushNotificationStudent([FromBody] PushNotificationStudentsRequest request)
        {
            var response = await _notificationService.GetPushNotificationStudent(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetPushNotificationEmployee")]
        public async Task<IActionResult> GetPushNotificationEmployee([FromBody] PushNotificationEmployeesRequest request)
        {
            var response = await _notificationService.GetPushNotificationEmployee(request);
            return StatusCode(response.StatusCode, response);
        }

        //[HttpPost("GetNotificationReport")]
        //public async Task<IActionResult> GetNotificationReport([FromBody] GetNotificationReportRequest request)
        //{
        //    var response = await _notificationService.GetNotificationReport(request);
        //    return StatusCode(response.StatusCode, response);
        //}
         

        [HttpPost("GetNotificationStudentReport")]
        public async Task<IActionResult> GetNotificationStudentReport([FromBody] GetNotificationStudentReportRequest request)
        {
            var response = await _notificationService.GetNotificationStudentReport(request);
            return StatusCode(response.StatusCode, response);
        }
         

        [HttpPost("GetNotificationStudentReportExport")]
        public async Task<IActionResult> GetNotificationStudentReportExport([FromBody] GetNotificationStudentReportExportRequest request)
        {
            // Get the export file content
            var response = await _notificationService.GetNotificationStudentReportExport(request);

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

                    // Determine the MIME type and filename based on ExportType
                    string mimeType;
                    string fileName;
                    if (request.ExportType == 1)
                    {
                        mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        fileName = "NotificationReport.xlsx";
                    }
                    else if (request.ExportType == 2)
                    {
                        mimeType = "text/csv";
                        fileName = "NotificationReport.csv";
                    }
                    else
                    {
                        return BadRequest("Invalid ExportType.");
                    }

                    // Return the file as a downloadable response
                    return File(fileBytes, mimeType, fileName);
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

        [HttpPost("GetNotificationEmployeeReport")]
        public async Task<IActionResult> GetNotificationEmployeeReport([FromBody] GetNotificationEmployeeReportRequest request)
        {
            var response = await _notificationService.GetNotificationEmployeeReport(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("GetNotificationEmployeeReportExport")]
        public async Task<IActionResult> GetNotificationEmployeeReportExport([FromBody] GetNotificationEmployeeReportExportRequest request)
        {
            // Get the export file content
            var response = await _notificationService.GetNotificationEmployeeReportExport(request);

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

                    // Determine the MIME type and filename based on ExportType
                    string mimeType;
                    string fileName;
                    if (request.ExportType == 1)
                    {
                        mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        fileName = "NotificationEmployeeReport.xlsx";
                    }
                    else if (request.ExportType == 2)
                    {
                        mimeType = "text/csv";
                        fileName = "NotificationEmployeeReport.csv";
                    }
                    else
                    {
                        return BadRequest("Invalid ExportType.");
                    }

                    // Return the file as a downloadable response
                    return File(fileBytes, mimeType, fileName);
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


        [HttpPost("SendPushNotificationStudent")]
        public async Task<IActionResult> SendPushNotificationStudent([FromBody] SendPushNotificationStudentRequest request)
        {
            var response = await _notificationService.SendPushNotificationStudent(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("SendPushNotificationEmployee")]
        public async Task<IActionResult> SendPushNotificationEmployee([FromBody] SendPushNotificationEmployeeRequest request)
        {
            var response = await _notificationService.SendPushNotificationEmployee(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("UpdatePushNotificationStudentStatus")]
        public async Task<IActionResult> UpdatePushNotificationStudentStatus([FromBody] UpdatePushNotificationStudentStatusRequest request)
        {
            var response = await _notificationService.UpdatePushNotificationStudentStatus(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("UpdatePushNotificationEmployeeStatus")]
        public async Task<IActionResult> UpdatePushNotificationEmployeeStatus([FromBody] UpdatePushNotificationEmployeeStatusRequest request)
        {
            var response = await _notificationService.UpdatePushNotificationEmployeeStatus(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("CreatePushNotificationTemplate")]
        public async Task<IActionResult> CreatePushNotificationTemplate([FromBody] CreatePushNotificationTemplate request)
        {
            var response = await _notificationService.CreatePushNotificationTemplate(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("GetAllPushNotificationTemplate")]
        public async Task<IActionResult> GetAllPushNotificationTemplate([FromBody] GetAllPushNotificationTemplateRequest request)
        {
            var response = await _notificationService.GetAllPushNotificationTemplate(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllPushNotificationTemplateExport")]
        public async Task<IActionResult> GetAllPushNotificationTemplateExport([FromBody] GetAllPushNotificationTemplateExportRequest request)
        {
            var response = await _notificationService.GetAllPushNotificationTemplateExport(request);
            if (response.Success)
            {
                // Return file based on ExportType
                var memoryStream = new MemoryStream();
                if (request.ExportType == 1)
                {
                    // Generate Excel
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets.Add("PushNotificationTemplates");
                        worksheet.Cells.LoadFromCollection(response.Data, true);
                        package.Save();
                    }

                    memoryStream.Position = 0;
                    return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PushNotificationTemplates.xlsx");
                }
                else if (request.ExportType == 2)
                {
                    // Generate CSV
                    var csv = string.Join(",", new[] { "TemplateName", "TemplateMessage" }) + "\n";
                    foreach (var template in response.Data)
                    {
                        csv += $"{template.TemplateName.Replace(",","")},{template.TemplateMessage.Replace(",", "")}\n";
                    }

                    var csvBytes = System.Text.Encoding.UTF8.GetBytes(csv);
                    memoryStream.Write(csvBytes, 0, csvBytes.Length);
                    memoryStream.Position = 0;
                    return File(memoryStream, "text/csv", "PushNotificationTemplates.csv");
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
