using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.Requests.WhatsApp;
using Communication_API.DTOs.Responses.SMS;
using Communication_API.DTOs.Responses.WhatsApp;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.WhatsApp;

namespace Communication_API.Services.Interfaces.WhatsApp
{
    public interface IWhatsAppService
    {
        Task<ServiceResponse<string>> Setup(SetupWhatsAppRequest request);
        Task<ServiceResponse<WhatsAppConfiguration>> GetBalance(int VendorID);
        Task<ServiceResponse<string>> AddUpdateTemplate(AddUpdateWhatsAppTemplateRequest request);
        Task<ServiceResponse<List<WhatsAppTemplate>>> GetWhatsAppTemplate(GetWhatsAppTemplateRequest request);
        Task<ServiceResponse<List<GetWhatsAppTemplateExportResponse>>> GetWhatsAppTemplateExport(GetWhatsAppTemplateExportRequest request);
        Task<ServiceResponse<string>> Send(SendWhatsAppRequest request);
        Task<ServiceResponse<List<WhatsAppReportResponse>>> GetWhatsAppReport(GetWhatsAppReportRequest request);
        Task<ServiceResponse<string>> SendWhatsAppToStudents(SendWhatsAppStudentRequest request);
        Task<ServiceResponse<string>> SendWhatsAppToEmployees(SendWhatsAppEmployeeRequest request);
        Task<ServiceResponse<string>> UpdateWhatsAppStudentStatus(UpdateWhatsAppStudentStatusRequest request);
        Task<ServiceResponse<string>> UpdateWhatsAppEmployeeStatus(UpdateWhatsAppEmployeeStatusRequest request);
        Task<ServiceResponse<List<WhatsAppTemplateDDLResponse>>> GetWhatsAppTemplateDDL(WhatsAppTemplateDDLRequest request);
        Task<ServiceResponse<List<WhatsAppStudentReportsResponse>>> GetWhatsAppStudentReport(GetWhatsAppStudentReportRequest request);
        Task<ServiceResponse<string>> GetWhatsAppStudentReportExport(WhatsAppStudentReportExportRequest request);
        Task<ServiceResponse<List<WhatsAppEmployeeReportsResponse>>> GetWhatsAppEmployeeReport(GetWhatsAppEmployeeReportRequest request);
        Task<ServiceResponse<string>> GetWhatsAppEmployeeReportExport(WhatsAppEmployeeReportExportRequest request);
    }
}
