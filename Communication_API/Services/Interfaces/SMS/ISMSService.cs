using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.SMS;

namespace Communication_API.Services.Interfaces.SMS
{
    public interface ISMSService
    {
        Task<ServiceResponse<string>> SetupSMSConfiguration(SetupSMSConfigurationRequest request);
        Task<ServiceResponse<SMSBalance>> GetSMSBalance(int VendorID);
        Task<ServiceResponse<string>> CreateSMSTemplate(CreateSMSTemplateRequest request);
        Task<ServiceResponse<List<SMSTemplate>>> GetAllSMSTemplate(GetAllSMSTemplateRequest request);
        Task<ServiceResponse<string>> SendNewSMS(SendNewSMSRequest request);
        Task<ServiceResponse<List<NotificationReport>>> GetSMSReport(GetSMSReportRequest request);
        Task<ServiceResponse<string>> SendSMSStudent(SendSMSStudentRequest request);
        Task<ServiceResponse<string>> SendSMSEmployee(SendSMSEmployeeRequest request);
        Task<ServiceResponse<string>> UpdateSMSStudentStatus(UpdateSMSStudentStatusRequest request);
        Task<ServiceResponse<string>> UpdateSMSEmployeeStatus(UpdateSMSEmployeeStatusRequest request);


    }
}
