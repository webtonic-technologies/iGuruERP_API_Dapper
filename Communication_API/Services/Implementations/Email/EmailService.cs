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

        public async Task<ServiceResponse<string>> SendEmailToStudents(SendEmailStudentRequest request)
        {
            try
            {
                // Parse EmailDate from string to DateTime
                DateTime emailDate = DateTime.ParseExact(request.EmailDate, "dd-MM-yyyy", null);

                // Iterate over each student ID and insert email data into the table
                foreach (var studentID in request.StudentIDs)
                {
                    await _emailRepository.InsertEmailForStudent(request.GroupID, request.InstituteID, studentID, request.EmailSubject, request.EmailBody, emailDate, 1); // Assuming EmailStatusID is 1
                }

                return new ServiceResponse<string>(true, "Email sent successfully to students.", "Emails sent successfully", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to send email", ex.Message, 500);
            }
        }
        public async Task<ServiceResponse<string>> SendEmailToEmployees(SendEmailEmployeeRequest request)
        {
            try
            {
                // Parse EmailDate from string to DateTime
                DateTime emailDate = DateTime.ParseExact(request.EmailDate, "dd-MM-yyyy", null);

                // Iterate over each employee ID and insert email data into the table
                foreach (var employeeID in request.EmployeeIDs)
                {
                    await _emailRepository.InsertEmailForEmployee(request.GroupID, request.InstituteID, employeeID, request.EmailSubject, request.EmailBody, emailDate, 1); // Assuming EmailStatusID is 1
                }

                return new ServiceResponse<string>(true, "Email sent successfully to employees.", "Emails sent successfully", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to send email", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<string>> UpdateEmailStudentStatus(UpdateEmailStudentStatusRequest request)
        {
            try
            {
                // Update the email status for the student
                await _emailRepository.UpdateEmailStatusForStudent(request.GroupID, request.InstituteID, request.StudentID, request.EmailStatusID);

                return new ServiceResponse<string>(true, "Email status updated successfully.", "Email status updated", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to update email status", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<string>> UpdateEmailEmployeeStatus(UpdateEmailEmployeeStatusRequest request)
        {
            try
            {
                // Update the email status for the employee
                await _emailRepository.UpdateEmailStatusForEmployee(request.GroupID, request.InstituteID, request.EmployeeID, request.EmailStatusID);

                return new ServiceResponse<string>(true, "Email status updated successfully.", "Email status updated", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to update email status", ex.Message, 500);
            }
        }

    }
}
