using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Responses;
using SiteAdmin_API.DTOs.ServiceResponse;
using System.Threading.Tasks;

namespace SiteAdmin_API.Services.Interfaces
{
    public interface ISMSService
    {
        Task<ServiceResponse<string>> AddUpdateSMSVendor(AddUpdateSMSVendorRequest request);
        Task<ServiceResponse<IEnumerable<GetAllSMSVendorResponse>>> GetAllSMSVendor();
        Task<ServiceResponse<GetSMSVendorByIDResponse>> GetSMSVendorByID(int SMSVendorID);
        Task<ServiceResponse<string>> DeleteSMSVendor(DeleteSMSVendorRequest request);
        Task<ServiceResponse<string>> AddUpdateSMSPlan(AddUpdateSMSPlanRequest request);
        Task<ServiceResponse<IEnumerable<GetAllSMSPlanResponse>>> GetAllSMSPlan(GetAllSMSPlanRequest request);
        Task<ServiceResponse<GetSMSPlanByIDResponse>> GetSMSPlanByID(int SMSVendorID);
        Task<ServiceResponse<string>> DeleteSMSPlan(int rateID);
        Task<ServiceResponse<string>> CreateSMSOrder(CreateSMSOrderRequest request);
        Task<ServiceResponse<IEnumerable<GetSMSOrderResponse>>> GetSMSOrder(GetSMSOrderRequest request);

    }
}
