using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Responses;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Repository.Interfaces;
using SiteAdmin_API.Services.Interfaces;
using System.Threading.Tasks;

namespace SiteAdmin_API.Services.Implementations
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly IWhatsAppRepository _whatsAppRepository;

        public WhatsAppService(IWhatsAppRepository whatsAppRepository)
        {
            _whatsAppRepository = whatsAppRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateWhatsAppVendor(AddUpdateWhatsAppVendorRequest request)
        {
            return await _whatsAppRepository.AddUpdateWhatsAppVendor(request);
        }

        public async Task<ServiceResponse<IEnumerable<GetAllWhatsAppVendorResponse>>> GetAllWhatsAppVendor()
        {
            return await _whatsAppRepository.GetAllWhatsAppVendor();
        }

        public async Task<ServiceResponse<GetWhatsAppVendorByIDResponse>> GetWhatsAppVendorByID(int WhatsAppVendorID)
        {
            return await _whatsAppRepository.GetWhatsAppVendorByID(WhatsAppVendorID);
        }
        public async Task<ServiceResponse<string>> DeleteWhatsAppVendor(DeleteWhatsAppVendorRequest request)
        {
            return await _whatsAppRepository.DeleteWhatsAppVendor(request.WhatsAppVendorID);
        }

        public async Task<ServiceResponse<string>> AddUpdateWhatsAppPlan(AddUpdateWhatsAppPlanRequest request)
        {
            return await _whatsAppRepository.AddUpdateWhatsAppPlan(request);
        }

        public async Task<ServiceResponse<IEnumerable<GetAllWhatsAppPlanResponse>>> GetAllWhatsAppPlan(GetAllWhatsAppPlanRequest request)
        {
            return await _whatsAppRepository.GetAllWhatsAppPlan(request);
        }
        public async Task<ServiceResponse<GetWhatsAppPlanByIDResponse>> GetWhatsAppPlanByID(int WhatsAppVendorID)
        {
            return await _whatsAppRepository.GetWhatsAppPlanByID(WhatsAppVendorID);
        }
        public async Task<ServiceResponse<string>> DeleteWhatsAppPlan(DeleteWhatsAppPlanRequest request)
        {
            return await _whatsAppRepository.DeleteWhatsAppPlan(request.RateID);
        }

        public async Task<ServiceResponse<string>> CreateWhatsAppOrder(CreateWhatsAppOrderRequest request)
        {
            return await _whatsAppRepository.CreateWhatsAppOrder(request);
        }
        public async Task<ServiceResponse<IEnumerable<GetWhatsAppOrderResponse>>> GetWhatsAppOrder(GetWhatsAppOrderRequest request)
        {
            return await _whatsAppRepository.GetWhatsAppOrder(request);
        }
    }
}
