using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;

namespace FeesManagement_API.Services.Implementations
{
    public class FeeCollectionService : IFeeCollectionService
    {
        private readonly IFeeCollectionRepository _feeCollectionRepository;

        public FeeCollectionService(IFeeCollectionRepository feeCollectionRepository)
        {
            _feeCollectionRepository = feeCollectionRepository;
        }

        public ServiceResponse<GetFeeResponse> GetFee(GetFeeRequest request)
        {
            return _feeCollectionRepository.GetFeesCollection(request);
        }
        public ServiceResponse<bool> SubmitPayment(SubmitPaymentRequest request)
        {
            return _feeCollectionRepository.SubmitPayment(request);
        }

    }
}
