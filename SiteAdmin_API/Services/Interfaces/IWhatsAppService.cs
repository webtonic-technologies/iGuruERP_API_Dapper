using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Responses;
using SiteAdmin_API.DTOs.ServiceResponse;


namespace SiteAdmin_API.Services.Interfaces
{
    public interface IWhatsAppService
    {
        Task<ServiceResponse<string>> AddUpdateWhatsAppVendor(AddUpdateWhatsAppVendorRequest request);
        Task<ServiceResponse<IEnumerable<GetAllWhatsAppVendorResponse>>> GetAllWhatsAppVendor();
        Task<ServiceResponse<GetWhatsAppVendorByIDResponse>> GetWhatsAppVendorByID(int WhatsAppVendorID);
        Task<ServiceResponse<string>> DeleteWhatsAppVendor(DeleteWhatsAppVendorRequest request);
        Task<ServiceResponse<string>> AddUpdateWhatsAppPlan(AddUpdateWhatsAppPlanRequest request);
        Task<ServiceResponse<IEnumerable<GetAllWhatsAppPlanResponse>>> GetAllWhatsAppPlan(GetAllWhatsAppPlanRequest request);
        Task<ServiceResponse<GetWhatsAppPlanByIDResponse>> GetWhatsAppPlanByID(int WhatsAppVendorID);
        Task<ServiceResponse<string>> DeleteWhatsAppPlan(DeleteWhatsAppPlanRequest request);
        Task<ServiceResponse<string>> CreateWhatsAppOrder(CreateWhatsAppOrderRequest request);
        Task<ServiceResponse<IEnumerable<GetWhatsAppOrderResponse>>> GetWhatsAppOrder(GetWhatsAppOrderRequest request);

    }
}
