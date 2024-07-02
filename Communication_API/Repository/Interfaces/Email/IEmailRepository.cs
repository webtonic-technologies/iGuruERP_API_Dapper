using Communication_API.DTOs.Requests.Email;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Email;

namespace Communication_API.Repository.Interfaces.Email
{
    public interface IEmailRepository
    {
        Task<ServiceResponse<string>> ConfigureEmail(ConfigureEmailRequest request);
        Task<ServiceResponse<string>> SendNewEmail(SendNewEmailRequest request);
        Task<ServiceResponse<List<EmailReport>>> GetEmailReports(GetEmailReportsRequest request);
    }
}
