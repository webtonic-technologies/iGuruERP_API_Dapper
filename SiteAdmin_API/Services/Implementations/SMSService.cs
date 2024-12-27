using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Responses;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Repository.Interfaces;
using SiteAdmin_API.Services.Interfaces;
using System.Threading.Tasks;

namespace SiteAdmin_API.Services.Implementations
{
    public class SMSService : ISMSService
    {
        private readonly ISMSRepository _smsRepository;

        public SMSService(ISMSRepository smsRepository)
        {
            _smsRepository = smsRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateSMSVendor(AddUpdateSMSVendorRequest request)
        {
            return await _smsRepository.AddUpdateSMSVendor(request);
        }

        public async Task<ServiceResponse<IEnumerable<GetAllSMSVendorResponse>>> GetAllSMSVendor()
        {
            return await _smsRepository.GetAllSMSVendor();
        }

        public async Task<ServiceResponse<GetSMSVendorByIDResponse>> GetSMSVendorByID(int SMSVendorID)
        {
            return await _smsRepository.GetSMSVendorByID(SMSVendorID);
        }

        public async Task<ServiceResponse<string>> DeleteSMSVendor(DeleteSMSVendorRequest request)
        {
            return await _smsRepository.DeleteSMSVendor(request);
        }

        public async Task<ServiceResponse<string>> AddUpdateSMSPlan(AddUpdateSMSPlanRequest request)
        {
            return await _smsRepository.AddUpdateSMSPlan(request);
        }

        public async Task<ServiceResponse<IEnumerable<GetAllSMSPlanResponse>>> GetAllSMSPlan(GetAllSMSPlanRequest request)
        {
            var plans = await _smsRepository.GetAllSMSPlan(request);
            return new ServiceResponse<IEnumerable<GetAllSMSPlanResponse>>(true, "SMS Plans fetched successfully.", plans, 200);
        }
         
        public async Task<ServiceResponse<GetSMSPlanByIDResponse>> GetSMSPlanByID(int SMSVendorID)
        {
            return await _smsRepository.GetSMSPlanByID(SMSVendorID);
        }
        public async Task<ServiceResponse<string>> DeleteSMSPlan(int rateID)
        {
            return await _smsRepository.DeleteSMSPlan(rateID);
        }

        public async Task<ServiceResponse<string>> CreateSMSOrder(CreateSMSOrderRequest request)
        {
            return await _smsRepository.CreateSMSOrder(request);
        }
        public async Task<ServiceResponse<IEnumerable<GetSMSOrderResponse>>> GetSMSOrder(GetSMSOrderRequest request)
        {
            return await _smsRepository.GetSMSOrder(request.StartDate, request.EndDate, request.StatusID);
        }

    }
}
