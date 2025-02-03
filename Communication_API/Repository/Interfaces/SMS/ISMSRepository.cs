using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Responses;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.SMS;

namespace Communication_API.Repository.Interfaces.SMS
{
    public interface ISMSRepository
    {
        Task<ServiceResponse<string>> SetupSMSConfiguration(SetupSMSConfigurationRequest request);
        Task<ServiceResponse<SMSBalance>> GetSMSBalance(int VendorID);
        Task<ServiceResponse<string>> CreateSMSTemplate(CreateSMSTemplateRequest request);
        Task<ServiceResponse<List<SMSTemplate>>> GetAllSMSTemplate(GetAllSMSTemplateRequest request);
        Task<List<SMSTemplateExportResponse>> GetAllSMSTemplateExport(int instituteID);

        Task<ServiceResponse<string>> SendNewSMS(SendNewSMSRequest request);
        Task<ServiceResponse<List<SMSStudentReportsResponse>>> GetSMSStudentReport(GetSMSStudentReportRequest request);
        Task<List<SMSStudentReportExportResponse>> GetSMSStudentReportData(SMSStudentReportExportRequest request); 
        Task<ServiceResponse<List<SMSEmployeeReportsResponse>>> GetSMSEmployeeReport(GetSMSEmployeeReportRequest request);
        Task<string> GetSMSEmployeeReportExport(SMSEmployeeReportExportRequest request); 
        Task InsertSMSForStudent(int groupID, int instituteID, int studentID, string smsMessage, DateTime smsDate, int smsStatusID);
        Task InsertSMSForEmployee(int groupID, int instituteID, int employeeID, string smsMessage, DateTime smsDate, int smsStatusID);
        Task UpdateSMSStudentStatus(int groupID, int instituteID, int studentID, int smsStatusID);
        Task UpdateSMSEmployeeStatus(int groupID, int instituteID, int employeeID, int smsStatusID);
        Task<ServiceResponse<List<SMSTemplateDDLResponse>>> GetSMSTemplateDDL(int instituteID); 
        Task<ServiceResponse<SMSPlanResponse>> GetSMSPlan(int SMSVendorID);
        Task<ServiceResponse<List<GetSMSTopUpHistoryResponse>>> GetSMSTopUpHistory(int instituteID);
        Task<List<GetSMSTopUpHistoryExportResponse>> GetSMSTopUpHistoryExport(int instituteID);

    }
}
