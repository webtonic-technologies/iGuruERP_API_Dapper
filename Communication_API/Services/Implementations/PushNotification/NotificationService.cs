using Communication_API.DTOs.Requests.PushNotification;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.PushNotification;
using Communication_API.Repository.Interfaces.PushNotification;
using Communication_API.Services.Interfaces.PushNotification;

namespace Communication_API.Services.Implementations.PushNotification
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<ServiceResponse<string>> TriggerNotification(TriggerNotificationRequest request)
        {
            return await _notificationRepository.TriggerNotification(request);
        }

        public async Task<ServiceResponse<List<Notification>>> GetNotificationReport(GetNotificationReportRequest request)
        {
            return await _notificationRepository.GetNotificationReport(request);
        }
    }
}
