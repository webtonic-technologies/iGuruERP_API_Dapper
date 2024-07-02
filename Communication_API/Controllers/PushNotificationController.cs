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

        [HttpPost("GetNotificationReport")]
        public async Task<IActionResult> GetNotificationReport([FromBody] GetNotificationReportRequest request)
        {
            var response = await _notificationService.GetNotificationReport(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
