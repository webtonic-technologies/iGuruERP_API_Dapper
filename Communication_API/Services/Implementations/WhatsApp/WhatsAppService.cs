using Communication_API.DTOs.Requests.WhatsApp;
using Communication_API.DTOs.Responses.WhatsApp;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.WhatsApp;
using Communication_API.Repository.Interfaces.WhatsApp;
using Communication_API.Services.Interfaces.WhatsApp;

namespace Communication_API.Services.Implementations.WhatsApp
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly IWhatsAppRepository _whatsAppRepository;

        public WhatsAppService(IWhatsAppRepository whatsAppRepository)
        {
            _whatsAppRepository = whatsAppRepository;
        }

        public async Task<ServiceResponse<string>> Setup(SetupWhatsAppRequest request)
        {
            return await _whatsAppRepository.Setup(request);
        }

        public async Task<ServiceResponse<WhatsAppConfiguration>> GetBalance(int VendorID)
        {
            return await _whatsAppRepository.GetBalance(VendorID);
        }

        public async Task<ServiceResponse<string>> AddUpdateTemplate(AddUpdateTemplateRequest request)
        {
            return await _whatsAppRepository.AddUpdateTemplate(request);
        }

        public async Task<ServiceResponse<List<WhatsAppTemplate>>> GetWhatsAppTemplate(GetWhatsAppTemplateRequest request)
        {
            return await _whatsAppRepository.GetWhatsAppTemplate(request);
        }

        public async Task<ServiceResponse<string>> Send(SendWhatsAppRequest request)
        {
            return await _whatsAppRepository.Send(request);
        }

        public async Task<ServiceResponse<List<WhatsAppReportResponse>>> GetWhatsAppReport(GetWhatsAppReportRequest request)
        {
            return await _whatsAppRepository.GetWhatsAppReport(request);
        }

        public async Task<ServiceResponse<string>> SendWhatsAppToStudents(SendWhatsAppStudentRequest request)
        {
            try
            {
                // Parse WhatsAppDate from string to DateTime
                DateTime whatsAppDate = DateTime.ParseExact(request.WhatsAppDate, "dd-MM-yyyy", null);

                // Step 1: Insert the WhatsApp message into tblWhatsAppStudent
                foreach (var studentID in request.StudentIDs)
                {
                    await _whatsAppRepository.InsertWhatsAppForStudent(request.GroupID, request.InstituteID, studentID, request.WhatsAppMessage, whatsAppDate, 0); // Assuming WhatsAppStatusID is 0 (Pending)
                }

                return new ServiceResponse<string>(true, "WhatsApp message sent successfully to students.", "WhatsApp message added/updated successfully", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to send WhatsApp message", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<string>> SendWhatsAppToEmployees(SendWhatsAppEmployeeRequest request)
        {
            try
            {
                // Parse WhatsAppDate from string to DateTime
                DateTime whatsAppDate = DateTime.ParseExact(request.WhatsAppDate, "dd-MM-yyyy", null);

                // Step 1: Insert the WhatsApp message into tblWhatsAppEmployee
                foreach (var employeeID in request.EmployeeIDs)
                {
                    await _whatsAppRepository.InsertWhatsAppForEmployee(request.GroupID, request.InstituteID, employeeID, request.WhatsAppMessage, whatsAppDate, 0); // Assuming WhatsAppStatusID is 0 (Pending)
                }

                return new ServiceResponse<string>(true, "WhatsApp message sent successfully to employees.", "WhatsApp message added/updated successfully", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to send WhatsApp message", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<string>> UpdateWhatsAppStudentStatus(UpdateWhatsAppStudentStatusRequest request)
        {
            try
            {
                // Step 1: Update the WhatsApp status for the student
                await _whatsAppRepository.UpdateWhatsAppStatusForStudent(request.GroupID, request.InstituteID, request.StudentID, request.WhatsAppStatusID);

                return new ServiceResponse<string>(true, "WhatsApp status updated successfully for student.", "WhatsApp status updated", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to update WhatsApp status", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<string>> UpdateWhatsAppEmployeeStatus(UpdateWhatsAppEmployeeStatusRequest request)
        {
            try
            {
                // Step 1: Update the WhatsApp status for the employee
                await _whatsAppRepository.UpdateWhatsAppStatusForEmployee(request.GroupID, request.InstituteID, request.EmployeeID, request.WhatsAppStatusID);

                return new ServiceResponse<string>(true, "WhatsApp status updated successfully for employee.", "WhatsApp status updated", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to update WhatsApp status", ex.Message, 500);
            }
        }
    }
}
