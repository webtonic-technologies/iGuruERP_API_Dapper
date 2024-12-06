using Communication_API.DTOs.Requests.PushNotification;
using Communication_API.DTOs.Responses.PushNotification;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.PushNotification;

namespace Communication_API.Repository.Interfaces.PushNotification
{
    public interface INotificationRepository
    {
        Task<ServiceResponse<string>> TriggerNotification(TriggerNotificationRequest request);
        Task<ServiceResponse<List<PushNotificationStudentsResponse>>> GetPushNotificationStudent(PushNotificationStudentsRequest request);
        Task<ServiceResponse<List<PushNotificationEmployeesResponse>>> GetPushNotificationEmployee(PushNotificationEmployeesRequest request); 
        Task<ServiceResponse<List<Notification>>> GetNotificationReport(GetNotificationReportRequest request);
        Task InsertPushNotificationForStudent(int groupID, int instituteID, int studentID, string message, DateTime notificationDate, int statusID);
        Task InsertPushNotificationForEmployee(int groupID, int instituteID, int employeeID, string message, DateTime notificationDate, int statusID);
        Task UpdatePushNotificationStudentStatus(int groupID, int instituteID, int studentID, int pushNotificationStatusID);
        Task UpdatePushNotificationEmployeeStatus(int groupID, int instituteID, int employeeID, int pushNotificationStatusID);

    }
}
