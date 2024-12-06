using Communication_API.DTOs.Requests.PushNotification;
using Communication_API.DTOs.Responses.PushNotification;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.PushNotification;

namespace Communication_API.Services.Interfaces.PushNotification
{
    public interface INotificationService
    {
        Task<ServiceResponse<string>> TriggerNotification(TriggerNotificationRequest request);
        Task<ServiceResponse<List<PushNotificationStudentsResponse>>> GetPushNotificationStudent(PushNotificationStudentsRequest request);
        Task<ServiceResponse<List<PushNotificationEmployeesResponse>>> GetPushNotificationEmployee(PushNotificationEmployeesRequest request); 
        Task<ServiceResponse<List<Notification>>> GetNotificationReport(GetNotificationReportRequest request);
        Task<ServiceResponse<string>> SendPushNotificationStudent(SendPushNotificationStudentRequest request);
        Task<ServiceResponse<string>> SendPushNotificationEmployee(SendPushNotificationEmployeeRequest request);
        Task<ServiceResponse<string>> UpdatePushNotificationStudentStatus(UpdatePushNotificationStudentStatusRequest request);
        Task<ServiceResponse<string>> UpdatePushNotificationEmployeeStatus(UpdatePushNotificationEmployeeStatusRequest request);


    }
}
