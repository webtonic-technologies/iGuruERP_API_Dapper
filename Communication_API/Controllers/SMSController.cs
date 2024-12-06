using Communication_API.DTOs.Requests.SMS;
using Communication_API.Services.Interfaces.SMS;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("SendNewSMS")]
        public async Task<IActionResult> SendNewSMS([FromBody] SendNewSMSRequest request)
        {
            var response = await _smsService.SendNewSMS(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetSMSReport")]
        public async Task<IActionResult> GetSMSReport([FromBody] GetSMSReportRequest request)
        {
            var response = await _smsService.GetSMSReport(request);
            return StatusCode(response.StatusCode, response);
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
    }
}
