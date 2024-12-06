using Communication_API.DTOs.Requests.WhatsApp;
using Communication_API.DTOs.Responses.WhatsApp;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.WhatsApp;

namespace Communication_API.Repository.Interfaces.WhatsApp
{
    public interface IWhatsAppRepository
    {
        Task<ServiceResponse<string>> Setup(SetupWhatsAppRequest request);
        Task<ServiceResponse<WhatsAppConfiguration>> GetBalance(int VendorID);
        Task<ServiceResponse<string>> AddUpdateTemplate(AddUpdateTemplateRequest request);
        Task<ServiceResponse<List<WhatsAppTemplate>>> GetWhatsAppTemplate(GetWhatsAppTemplateRequest request);
        Task<ServiceResponse<string>> Send(SendWhatsAppRequest request);
        Task<ServiceResponse<List<WhatsAppReportResponse>>> GetWhatsAppReport(GetWhatsAppReportRequest request);
        Task InsertWhatsAppForStudent(int groupID, int instituteID, int studentID, string whatsAppMessage, DateTime whatsAppDate, int whatsAppStatusID);
        Task InsertWhatsAppForEmployee(int groupID, int instituteID, int employeeID, string whatsAppMessage, DateTime whatsAppDate, int whatsAppStatusID);
        Task UpdateWhatsAppStatusForStudent(int groupID, int instituteID, int studentID, int whatsAppStatusID);
        Task UpdateWhatsAppStatusForEmployee(int groupID, int instituteID, int employeeID, int whatsAppStatusID);  
    }
}
