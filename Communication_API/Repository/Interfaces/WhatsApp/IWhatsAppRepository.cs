using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Requests.WhatsApp;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.Responses.WhatsApp;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.WhatsApp;

namespace Communication_API.Repository.Interfaces.WhatsApp
{
    public interface IWhatsAppRepository
    {
        Task<ServiceResponse<string>> Setup(SetupWhatsAppRequest request);
        Task<ServiceResponse<WhatsAppConfiguration>> GetBalance(int VendorID);
        Task<ServiceResponse<string>> AddUpdateTemplate(AddUpdateWhatsAppTemplateRequest request);
        Task<ServiceResponse<List<WhatsAppTemplate>>> GetWhatsAppTemplate(GetWhatsAppTemplateRequest request);
        Task<List<GetWhatsAppTemplateExportResponse>> GetWhatsAppTemplateExport(int instituteID);
        Task<ServiceResponse<string>> Send(SendWhatsAppRequest request);
        Task<ServiceResponse<List<WhatsAppReportResponse>>> GetWhatsAppReport(GetWhatsAppReportRequest request);
        Task InsertWhatsAppForStudent(int groupID, int instituteID, int studentID, string whatsAppMessage, DateTime whatsAppDate, int whatsAppStatusID, int SentBy);
        Task InsertWhatsAppForEmployee(int groupID, int instituteID, int employeeID, string whatsAppMessage, DateTime whatsAppDate, int whatsAppStatusID, int SentBy);
        Task UpdateWhatsAppStatusForStudent(int groupID, int instituteID, int studentID, int whatsAppStatusID);
        Task UpdateWhatsAppStatusForEmployee(int groupID, int instituteID, int employeeID, int whatsAppStatusID);
        Task<ServiceResponse<List<WhatsAppTemplateDDLResponse>>> GetWhatsAppTemplateDDL(int instituteID);
        Task<ServiceResponse<List<WhatsAppStudentReportsResponse>>> GetWhatsAppStudentReport(GetWhatsAppStudentReportRequest request);
        Task<List<WhatsAppStudentReportExportResponse>> GetWhatsAppStudentReportData(WhatsAppStudentReportExportRequest request);
        Task<ServiceResponse<List<WhatsAppEmployeeReportsResponse>>> GetWhatsAppEmployeeReport(GetWhatsAppEmployeeReportRequest request);
        Task<string> GetWhatsAppEmployeeReportExport(WhatsAppEmployeeReportExportRequest request);
        Task<ServiceResponse<WhatsAppPlanResponse>> GetWhatsAppPlan(int WhatsAppVendorID);
        Task<ServiceResponse<List<GetWhatsAppTopUpHistoryResponse>>> GetWhatsAppTopUpHistory(int instituteID);
        Task<List<GetWhatsAppTopUpHistoryExportResponse>> GetWhatsAppTopUpHistoryExport(int instituteID);

    }
}
