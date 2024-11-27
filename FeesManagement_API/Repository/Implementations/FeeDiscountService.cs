using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;

namespace FeesManagement_API.Services.Implementations
{
    public class FeeDiscountService : IFeeDiscountService
    {
        private readonly IFeeDiscountRepository _feeDiscountRepository;

        public FeeDiscountService(IFeeDiscountRepository feeDiscountRepository)
        {
            _feeDiscountRepository = feeDiscountRepository;
        }

        public ServiceResponse<bool> ApplyDiscount(SubmitFeeDiscountRequest request)
        {
            return _feeDiscountRepository.ApplyDiscount(request);
        }
    }
}
