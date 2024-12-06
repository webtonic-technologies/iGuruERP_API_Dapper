using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.SMS;
using Communication_API.Repository.Interfaces.SMS;
using Communication_API.Services.Interfaces.SMS;

namespace Communication_API.Services.Implementations.SMS
{
    public class SMSService : ISMSService
    {
        private readonly ISMSRepository _smsRepository;

        public SMSService(ISMSRepository smsRepository)
        {
            _smsRepository = smsRepository;
        }

        public async Task<ServiceResponse<string>> SetupSMSConfiguration(SetupSMSConfigurationRequest request)
        {
            return await _smsRepository.SetupSMSConfiguration(request);
        }

        public async Task<ServiceResponse<SMSBalance>> GetSMSBalance(int VendorID)
        {
            return await _smsRepository.GetSMSBalance(VendorID);
        }

        public async Task<ServiceResponse<string>> CreateSMSTemplate(CreateSMSTemplateRequest request)
        {
            return await _smsRepository.CreateSMSTemplate(request);
        }

        public async Task<ServiceResponse<List<SMSTemplate>>> GetAllSMSTemplate(GetAllSMSTemplateRequest request)
        {
            return await _smsRepository.GetAllSMSTemplate(request);
        }

        public async Task<ServiceResponse<string>> SendNewSMS(SendNewSMSRequest request)
        {
            return await _smsRepository.SendNewSMS(request);
        }

        public async Task<ServiceResponse<List<NotificationReport>>> GetSMSReport(GetSMSReportRequest request)
        {
            return await _smsRepository.GetSMSReport(request);
        }

        public async Task<ServiceResponse<string>> SendSMSStudent(SendSMSStudentRequest request)
        {
            try
            {
                // Convert SMSDate from string to DateTime
                DateTime smsDate = DateTime.ParseExact(request.SMSDate, "dd-MM-yyyy", null);

                // Iterate over each student ID and insert SMS data into the table
                foreach (var studentID in request.StudentIDs)
                {
                    await _smsRepository.InsertSMSForStudent(request.GroupID, request.InstituteID, studentID, request.SMSMessage, smsDate, 1); // Assuming SMSStatusID is 1
                }

                return new ServiceResponse<string>(true, "SMS sent successfully to students.", "SMS sent successfully", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to send SMS", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<string>> SendSMSEmployee(SendSMSEmployeeRequest request)
        {
            try
            {
                // Convert SMSDate from string to DateTime
                DateTime smsDate = DateTime.ParseExact(request.SMSDate, "dd-MM-yyyy", null);

                // Iterate over each employee ID and insert SMS data into the table
                foreach (var employeeID in request.EmployeeIDs)
                {
                    await _smsRepository.InsertSMSForEmployee(request.GroupID, request.InstituteID, employeeID, request.SMSMessage, smsDate, 1); // Assuming SMSStatusID is 1
                }

                return new ServiceResponse<string>(true, "SMS sent successfully to employees.", "SMS sent successfully", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to send SMS", ex.Message, 500);
            }
        }


        public async Task<ServiceResponse<string>> UpdateSMSStudentStatus(UpdateSMSStudentStatusRequest request)
        {
            try
            {
                // Call the repository method to update the SMS status in the database
                await _smsRepository.UpdateSMSStudentStatus(request.GroupID, request.InstituteID, request.StudentID, request.SMSStatusID);

                return new ServiceResponse<string>(true, "SMS status updated successfully.", "SMS status updated", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to update SMS status", ex.Message, 500);
            }
        }

        public async Task<ServiceResponse<string>> UpdateSMSEmployeeStatus(UpdateSMSEmployeeStatusRequest request)
        {
            try
            {
                // Call the repository method to update the SMS status in the database
                await _smsRepository.UpdateSMSEmployeeStatus(request.GroupID, request.InstituteID, request.EmployeeID, request.SMSStatusID);

                return new ServiceResponse<string>(true, "SMS status updated successfully.", "SMS status updated", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to update SMS status", ex.Message, 500);
            }
        }
    }
}
