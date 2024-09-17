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
    }
}
