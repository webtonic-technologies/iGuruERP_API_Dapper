using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Responses;
using SiteAdmin_API.DTOs.ServiceResponse;


namespace SiteAdmin_API.Repository.Interfaces
{
    public interface IWhatsAppRepository
    {
        Task<ServiceResponse<string>> AddUpdateWhatsAppVendor(AddUpdateWhatsAppVendorRequest request);
        Task<ServiceResponse<IEnumerable<GetAllWhatsAppVendorResponse>>> GetAllWhatsAppVendor();
        Task<ServiceResponse<GetWhatsAppVendorByIDResponse>> GetWhatsAppVendorByID(int WhatsAppVendorID);
        Task<ServiceResponse<string>> DeleteWhatsAppVendor(int WhatsAppVendorID);
        Task<ServiceResponse<string>> AddUpdateWhatsAppPlan(AddUpdateWhatsAppPlanRequest request);
        Task<ServiceResponse<IEnumerable<GetAllWhatsAppPlanResponse>>> GetAllWhatsAppPlan(GetAllWhatsAppPlanRequest request);
        Task<ServiceResponse<GetWhatsAppPlanByIDResponse>> GetWhatsAppPlanByID(int WhatsAppVendorID);
        Task<ServiceResponse<string>> DeleteWhatsAppPlan(int RateID);
        Task<ServiceResponse<string>> CreateWhatsAppOrder(CreateWhatsAppOrderRequest request);
        Task<ServiceResponse<IEnumerable<GetWhatsAppOrderResponse>>> GetWhatsAppOrder(GetWhatsAppOrderRequest request);


    }
}
