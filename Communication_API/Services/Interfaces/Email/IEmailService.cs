﻿using Communication_API.DTOs.Requests.Email;
using Communication_API.DTOs.Responses.Email;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Email;

namespace Communication_API.Services.Interfaces.Email
{
    public interface IEmailService
    {
        Task<ServiceResponse<string>> ConfigureEmail(ConfigureEmailRequest request);
        Task<ServiceResponse<string>> SendNewEmail(SendNewEmailRequest request);
        Task<ServiceResponse<List<EmailReportResponse>>> GetEmailReports(GetEmailReportsRequest request); // Change here
    }
}
