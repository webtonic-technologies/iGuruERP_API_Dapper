using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;

namespace FeesManagement_API.Services.Implementations
{
    public class FeeReceiptService : IFeeReceiptService
    {
        private readonly IFeeReceiptRepository _feeReceiptRepository;

        public FeeReceiptService(IFeeReceiptRepository feeReceiptRepository)
        {
            _feeReceiptRepository = feeReceiptRepository;
        } 

        public async Task<ServiceResponse<IEnumerable<GetReceiptComponentResponse>>> GetReceiptComponents(GetReceiptComponentRequest request)
        {
            return await _feeReceiptRepository.GetReceiptComponents(request);
        }

        public async Task<ServiceResponse<IEnumerable<GetReceiptPropertyResponse>>> GetReceiptProperties(GetReceiptPropertyRequest request)
        {
            return await _feeReceiptRepository.GetReceiptProperties(request);
        }

        public async Task<ServiceResponse<IEnumerable<GetReceiptLayoutResponse>>> GetReceiptLayouts()
        {
            return await _feeReceiptRepository.GetReceiptLayouts();
        }

        public async Task<ServiceResponse<IEnumerable<GetReceiptTypeResponse>>> GetReceiptTypes()
        {
            return await _feeReceiptRepository.GetReceiptTypes();
        }

        public async Task<ServiceResponse<bool>> ConfigureReceipt(ConfigureReceiptRequest request)
        {
            return await _feeReceiptRepository.ConfigureReceipt(request);
        }

    }
}
