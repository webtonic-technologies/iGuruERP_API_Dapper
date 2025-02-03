using Communication_API.DTOs.Requests;
using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Responses;
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
        Task<ServiceResponse<List<SMSTemplateExportResponse>>> GetAllSMSTemplateExport(SMSTemplateExportRequest request);
        Task<ServiceResponse<string>> SendNewSMS(SendNewSMSRequest request);
        Task<ServiceResponse<List<SMSStudentReportsResponse>>> GetSMSStudentReport(GetSMSStudentReportRequest request);
        Task<ServiceResponse<string>> GetSMSStudentReportExport(SMSStudentReportExportRequest request); 
        Task<ServiceResponse<List<SMSEmployeeReportsResponse>>> GetSMSEmployeeReport(GetSMSEmployeeReportRequest request);
        Task<ServiceResponse<string>> GetSMSEmployeeReportExport(SMSEmployeeReportExportRequest request);
        Task<ServiceResponse<string>> SendSMSStudent(SendSMSStudentRequest request);
        Task<ServiceResponse<string>> SendSMSEmployee(SendSMSEmployeeRequest request);
        Task<ServiceResponse<string>> UpdateSMSStudentStatus(UpdateSMSStudentStatusRequest request);
        Task<ServiceResponse<string>> UpdateSMSEmployeeStatus(UpdateSMSEmployeeStatusRequest request);
        Task<ServiceResponse<List<SMSTemplateDDLResponse>>> GetSMSTemplateDDL(SMSTemplateDDLRequest request);
        Task<ServiceResponse<SMSPlanResponse>> GetSMSPlan(int SMSVendorID);  // Add this line
        Task<ServiceResponse<List<GetSMSTopUpHistoryResponse>>> GetSMSTopUpHistory(GetSMSTopUpHistoryRequest request);
        Task<ServiceResponse<List<GetSMSTopUpHistoryExportResponse>>> GetSMSTopUpHistoryExport(GetSMSTopUpHistoryExportRequest request);

    }
}
