using Communication_API.DTOs.Requests.PushNotification;
using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Responses.PushNotification;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.PushNotification;
using Communication_API.Models.SMS;

namespace Communication_API.Services.Interfaces.PushNotification
{
    public interface INotificationService
    {
        Task<ServiceResponse<string>> TriggerNotification(TriggerNotificationRequest request);
        Task<ServiceResponse<List<PushNotificationStudentsResponse>>> GetPushNotificationStudent(PushNotificationStudentsRequest request);
        Task<ServiceResponse<List<PushNotificationEmployeesResponse>>> GetPushNotificationEmployee(PushNotificationEmployeesRequest request); 
        Task<ServiceResponse<List<GetNotificationStudentReportResponse>>> GetNotificationStudentReport(GetNotificationStudentReportRequest request);
        Task<ServiceResponse<string>> GetNotificationStudentReportExport(GetNotificationStudentReportExportRequest request);
        Task<ServiceResponse<List<GetNotificationEmployeeReportResponse>>> GetNotificationEmployeeReport(GetNotificationEmployeeReportRequest request);
        Task<ServiceResponse<string>> GetNotificationEmployeeReportExport(GetNotificationEmployeeReportExportRequest request); 
        Task<ServiceResponse<string>> SendPushNotificationStudent(SendPushNotificationStudentRequest request);
        Task<ServiceResponse<string>> SendPushNotificationEmployee(SendPushNotificationEmployeeRequest request);
        Task<ServiceResponse<string>> UpdatePushNotificationStudentStatus(UpdatePushNotificationStudentStatusRequest request);
        Task<ServiceResponse<string>> UpdatePushNotificationEmployeeStatus(UpdatePushNotificationEmployeeStatusRequest request);
        Task<ServiceResponse<string>> CreatePushNotificationTemplate(CreatePushNotificationTemplate request);
        Task<ServiceResponse<List<GetAllPushNotificationTemplateResponse>>> GetAllPushNotificationTemplate(GetAllPushNotificationTemplateRequest request);
        Task<ServiceResponse<List<GetAllPushNotificationTemplateExportResponse>>> GetAllPushNotificationTemplateExport(GetAllPushNotificationTemplateExportRequest request);
    }
}
