using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Responses;
using SiteAdmin_API.DTOs.ServiceResponse;
using System.Threading.Tasks;

namespace SiteAdmin_API.Repository.Interfaces
{
    public interface ISMSRepository
    {
        Task<ServiceResponse<string>> AddUpdateSMSVendor(AddUpdateSMSVendorRequest request);
        Task<ServiceResponse<IEnumerable<GetAllSMSVendorResponse>>> GetAllSMSVendor();
        Task<ServiceResponse<GetSMSVendorByIDResponse>> GetSMSVendorByID(int SMSVendorID);
        Task<ServiceResponse<string>> DeleteSMSVendor(DeleteSMSVendorRequest request);
        Task<ServiceResponse<string>> AddUpdateSMSPlan(AddUpdateSMSPlanRequest request);
        Task<IEnumerable<GetAllSMSPlanResponse>> GetAllSMSPlan(GetAllSMSPlanRequest request);
        Task<ServiceResponse<GetSMSPlanByIDResponse>> GetSMSPlanByID(int SMSVendorID);
        Task<ServiceResponse<string>> DeleteSMSPlan(int rateID);
        Task<ServiceResponse<string>> CreateSMSOrder(CreateSMSOrderRequest request);
        Task<ServiceResponse<IEnumerable<GetSMSOrderResponse>>> GetSMSOrder(DateTime startDate, DateTime endDate, int statusID);

    }
}
