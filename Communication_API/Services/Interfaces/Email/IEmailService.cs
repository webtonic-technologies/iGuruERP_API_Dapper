using Communication_API.DTOs.Requests.Email;
using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Responses.Email;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Email;

namespace Communication_API.Services.Interfaces.Email
{
    public interface IEmailService
    {
        Task<ServiceResponse<string>> ConfigureEmail(ConfigureEmailRequest request);
        Task<ServiceResponse<string>> SendNewEmail(SendNewEmailRequest request);
        Task<ServiceResponse<List<EmailReportResponse>>> GetEmailReports(GetEmailReportsRequest request); // Change here
        Task<ServiceResponse<string>> SendEmailToStudents(SendEmailStudentRequest request);
        Task<ServiceResponse<string>> SendEmailToEmployees(SendEmailEmployeeRequest request);
        Task<ServiceResponse<string>> UpdateEmailStudentStatus(UpdateEmailStudentStatusRequest request);
        Task<ServiceResponse<string>> UpdateEmailEmployeeStatus(UpdateEmailEmployeeStatusRequest request);
        Task<ServiceResponse<List<EmailStudentReportsResponse>>> GetEmailStudentReport(GetEmailStudentReportRequest request);
        Task<ServiceResponse<string>> GetEmailStudentReportExport(EmailStudentReportExportRequest request);
        Task<ServiceResponse<List<EmailEmployeeReportsResponse>>> GetEmailEmployeeReport(GetEmailEmployeeReportRequest request);
        Task<ServiceResponse<string>> GetEmailEmployeeReportExport(EmailEmployeeReportExportRequest request);
    }
}
