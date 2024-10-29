using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;

namespace FeesManagement_API.Services.Implementations
{
    public class FeeWaiverService : IFeeWaiverService
    {
        private readonly IFeeWaiverRepository _feeWaiverRepository;

        public FeeWaiverService(IFeeWaiverRepository feeWaiverRepository)
        {
            _feeWaiverRepository = feeWaiverRepository;
        }

        public ServiceResponse<bool> SubmitFeeWaiver(SubmitFeeWaiverRequest request)
        {
            // Here you can add any business logic or validation you need
            // For example: Validate the request data, log info, etc.

            return _feeWaiverRepository.SubmitFeeWaiver(request);
        }
    }
}
