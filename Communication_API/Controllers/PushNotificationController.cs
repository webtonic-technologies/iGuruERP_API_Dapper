using Communication_API.DTOs.Requests.PushNotification;
using Communication_API.Services.Interfaces.PushNotification;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("GetNotificationReport")]
        public async Task<IActionResult> GetNotificationReport([FromBody] GetNotificationReportRequest request)
        {
            var response = await _notificationService.GetNotificationReport(request);
            return StatusCode(response.StatusCode, response);
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
    }
}
