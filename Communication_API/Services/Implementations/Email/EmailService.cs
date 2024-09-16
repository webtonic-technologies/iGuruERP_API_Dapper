using Communication_API.DTOs.Requests.Email;
using Communication_API.DTOs.Responses.Email;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Email;
using Communication_API.Repository.Interfaces.Email;
using Communication_API.Services.Interfaces.Email;

namespace Communication_API.Services.Implementations.Email
{
    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _emailRepository;

        public EmailService(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<ServiceResponse<string>> ConfigureEmail(ConfigureEmailRequest request)
        {
            return await _emailRepository.ConfigureEmail(request);
        }

        public async Task<ServiceResponse<string>> SendNewEmail(SendNewEmailRequest request)
        {
            return await _emailRepository.SendNewEmail(request);
        }

        public async Task<ServiceResponse<List<EmailReportResponse>>> GetEmailReports(GetEmailReportsRequest request)
        {
            return await _emailRepository.GetEmailReports(request);
        }

    }
}
