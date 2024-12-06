using Communication_API.DTOs.Requests.Email;
using Communication_API.DTOs.Responses.Email;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Email;

namespace Communication_API.Repository.Interfaces.Email
{
    public interface IEmailRepository
    {
        Task<ServiceResponse<string>> ConfigureEmail(ConfigureEmailRequest request);
        Task<ServiceResponse<string>> SendNewEmail(SendNewEmailRequest request);
        Task<ServiceResponse<List<EmailReportResponse>>> GetEmailReports(GetEmailReportsRequest request); // Change here
        Task InsertEmailForStudent(int groupID, int instituteID, int studentID, string emailSubject, string emailBody, DateTime emailDate, int emailStatusID);
        Task InsertEmailForEmployee(int groupID, int instituteID, int employeeID, string emailSubject, string emailBody, DateTime emailDate, int emailStatusID);
        Task UpdateEmailStatusForStudent(int groupID, int instituteID, int studentID, int emailStatusID);
        Task UpdateEmailStatusForEmployee(int groupID, int instituteID, int employeeID, int emailStatusID);

    }
}
