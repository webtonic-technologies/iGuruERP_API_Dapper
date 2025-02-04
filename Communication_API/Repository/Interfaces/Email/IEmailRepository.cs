using Communication_API.DTOs.Requests.Email;
using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Responses.Email;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Email;

namespace Communication_API.Repository.Interfaces.Email
{
    public interface IEmailRepository
    {
        Task<ServiceResponse<string>> ConfigureEmail(ConfigureEmailRequest request);
        Task<ServiceResponse<string>> SendNewEmail(SendNewEmailRequest request);
        Task<ServiceResponse<List<EmailReportResponse>>> GetEmailReports(GetEmailReportsRequest request); // Change here
        Task InsertEmailForStudent(int groupID, int instituteID, int studentID, string emailSubject, string emailBody, DateTime emailDate, int emailStatusID, int SentBy);
        Task InsertEmailForEmployee(int groupID, int instituteID, int employeeID, string emailSubject, string emailBody, DateTime emailDate, int emailStatusID, int SentBy);
        Task UpdateEmailStatusForStudent(int groupID, int instituteID, int studentID, int emailStatusID);
        Task UpdateEmailStatusForEmployee(int groupID, int instituteID, int employeeID, int emailStatusID);
        Task<ServiceResponse<List<EmailStudentReportsResponse>>> GetEmailStudentReport(GetEmailStudentReportRequest request);
        Task<List<EmailStudentReportExportResponse>> GetEmailStudentReportData(EmailStudentReportExportRequest request);
        Task<ServiceResponse<List<EmailEmployeeReportsResponse>>> GetEmailEmployeeReport(GetEmailEmployeeReportRequest request);
        Task<ServiceResponse<string>> GetEmailEmployeeReportExport(EmailEmployeeReportExportRequest request);
    }
}
