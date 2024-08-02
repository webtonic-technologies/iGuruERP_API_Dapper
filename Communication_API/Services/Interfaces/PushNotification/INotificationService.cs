using Communication_API.DTOs.Requests.PushNotification;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.PushNotification;

namespace Communication_API.Services.Interfaces.PushNotification
{
    public interface INotificationService
    {
        Task<ServiceResponse<string>> TriggerNotification(TriggerNotificationRequest request);
        Task<ServiceResponse<List<Notification>>> GetNotificationReport(GetNotificationReportRequest request);
    }
}
