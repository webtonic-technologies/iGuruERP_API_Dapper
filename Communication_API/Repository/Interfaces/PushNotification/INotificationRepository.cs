using Communication_API.DTOs.Requests.PushNotification;
using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Responses.PushNotification;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.PushNotification;
using Communication_API.Models.SMS;

namespace Communication_API.Repository.Interfaces.PushNotification
{
    public interface INotificationRepository
    {
        Task<ServiceResponse<string>> TriggerNotification(TriggerNotificationRequest request);
        Task<ServiceResponse<List<PushNotificationStudentsResponse>>> GetPushNotificationStudent(PushNotificationStudentsRequest request);
        Task<ServiceResponse<List<PushNotificationEmployeesResponse>>> GetPushNotificationEmployee(PushNotificationEmployeesRequest request); 
        Task<ServiceResponse<List<GetNotificationStudentReportResponse>>> GetNotificationStudentReport(GetNotificationStudentReportRequest request);
        Task<List<GetNotificationStudentReportResponse>> GetNotificationStudentReportExport(GetNotificationStudentReportExportRequest request);
        Task<ServiceResponse<List<GetNotificationEmployeeReportResponse>>> GetNotificationEmployeeReport(GetNotificationEmployeeReportRequest request);
        Task<List<GetNotificationEmployeeReportResponse>> GetNotificationEmployeeReportExport(GetNotificationEmployeeReportExportRequest request);

        Task InsertPushNotificationForStudent(int groupID, int instituteID, int studentID, string message, DateTime notificationDate, int statusID, int SentBy);
        Task InsertPushNotificationForEmployee(int groupID, int instituteID, int employeeID, string message, DateTime notificationDate, int statusID, int SentBy);
        Task UpdatePushNotificationStudentStatus(int groupID, int instituteID, int studentID, int pushNotificationStatusID);
        Task UpdatePushNotificationEmployeeStatus(int groupID, int instituteID, int employeeID, int pushNotificationStatusID);
        Task<ServiceResponse<string>> CreatePushNotificationTemplate(CreatePushNotificationTemplate request);   
        Task<ServiceResponse<List<GetAllPushNotificationTemplateResponse>>> GetAllPushNotificationTemplate(GetAllPushNotificationTemplateRequest request);
        Task<List<GetAllPushNotificationTemplateExportResponse>> GetAllPushNotificationTemplateExport(int instituteID);
    }
}
