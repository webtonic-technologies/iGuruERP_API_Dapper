using Communication_API.DTOs.Requests.Email;
using Communication_API.DTOs.Requests.SMS;
using Communication_API.Services.Interfaces.Email;
using Microsoft.AspNetCore.Mvc;

namespace Communication_API.Controllers
{
    [Route("iGuru/Email/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("ConfigureEmail")]
        public async Task<IActionResult> ConfigureEmail([FromBody] ConfigureEmailRequest request)
        {
            var response = await _emailService.ConfigureEmail(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("SendNewEmail")]
        public async Task<IActionResult> SendNewEmail([FromBody] SendNewEmailRequest request)
        {
            var response = await _emailService.SendNewEmail(request);
            return StatusCode(response.StatusCode, response);
        }

        //[HttpPost("GetEmailReports")]
        //public async Task<IActionResult> GetEmailReports([FromBody] GetEmailReportsRequest request)
        //{
        //    var response = await _emailService.GetEmailReports(request);
        //    return StatusCode(response.StatusCode, response);
        //}

        [HttpPost("SendEmailStudent")]
        public async Task<IActionResult> SendEmailStudent(SendEmailStudentRequest request)
        {
            // Call the service layer to send email to students
            var response = await _emailService.SendEmailToStudents(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }

        [HttpPost("SendEmailEmployee")]
        public async Task<IActionResult> SendEmailEmployee(SendEmailEmployeeRequest request)
        {
            // Call the service layer to send email to employees
            var response = await _emailService.SendEmailToEmployees(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }

        [HttpPost("UpdateEmailStudentStatus")]
        public async Task<IActionResult> UpdateEmailStudentStatus(UpdateEmailStudentStatusRequest request)
        {
            // Call the service layer to update the email status for the student
            var response = await _emailService.UpdateEmailStudentStatus(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }

        [HttpPost("UpdateEmailEmployeeStatus")]
        public async Task<IActionResult> UpdateEmailEmployeeStatus(UpdateEmailEmployeeStatusRequest request)
        {
            // Call the service layer to update the email status for the employee
            var response = await _emailService.UpdateEmailEmployeeStatus(request);

            if (response.Success)
            {
                return Ok(response); // 200 OK response
            }

            return BadRequest(response); // 400 Bad Request response
        }

        [HttpPost("GetEmailStudentReport")]
        public async Task<IActionResult> GetEmailStudentReport([FromBody] GetEmailStudentReportRequest request)
        {
            var response = await _emailService.GetEmailStudentReport(request);
            return StatusCode(response.StatusCode, response);
        }



        [HttpPost("GetEmailStudentReportExport")]
        public async Task<IActionResult> GetSMSStudentReportExport([FromBody] EmailStudentReportExportRequest request)
        {
            // Get the export file content
            var response = await _emailService.GetEmailStudentReportExport(request);

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
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmailReport.xlsx");
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


        [HttpPost("GetEmailEmployeeReport")]
        public async Task<IActionResult> GetEmailEmployeeReport([FromBody] GetEmailEmployeeReportRequest request)
        {
            var response = await _emailService.GetEmailEmployeeReport(request);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("GetEmailEmployeeReportExport")]
        public async Task<IActionResult> GetEmailEmployeeReportExport([FromBody] EmailEmployeeReportExportRequest request)
        {
            var response = await _emailService.GetEmailEmployeeReportExport(request);

            if (response.Success)
            {
                string filePath = response.Data;
                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmailReport.xlsx");
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
    }
}
