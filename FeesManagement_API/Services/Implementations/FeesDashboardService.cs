using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Services.Implementations
{
    public class FeesDashboardService : IFeesDashboardService
    {
        private readonly IFeesDashboardRepository _feesDashboardRepository;

        public FeesDashboardService(IFeesDashboardRepository feesDashboardRepository)
        {
            _feesDashboardRepository = feesDashboardRepository;
        }

        public async Task<TotalAmountCollectedResponse> GetTotalAmountCollectedAsync(TotalAmountCollectedRequest request)
        {
            var totalAmount = await _feesDashboardRepository.GetTotalAmountCollectedAsync(request.InstituteID);
            return new TotalAmountCollectedResponse
            {
                TotalAmountCollected = totalAmount
            };
        }

        public async Task<ServiceResponse<TotalPendingAmountResponse>> GetTotalPendingAmountAsync(TotalPendingAmountRequest request)
        {
            var totalPendingAmount = await _feesDashboardRepository.GetTotalPendingAmountAsync(request.InstituteID);
            return new ServiceResponse<TotalPendingAmountResponse>(true, "Data retrieved successfully.", new TotalPendingAmountResponse
            {
                TotalPendingAmount = totalPendingAmount
            }, 200);
        }
        public async Task<ServiceResponse<List<HeadWiseCollectedAmountResponse>>> GetHeadWiseCollectedAmountAsync(HeadWiseCollectedAmountRequest request)
        {
            var amounts = await _feesDashboardRepository.GetHeadWiseCollectedAmountAsync(request);
            return new ServiceResponse<List<HeadWiseCollectedAmountResponse>>(true, "Data retrieved successfully.", amounts, 200);
        }
        public async Task<ServiceResponse<List<DayWiseResponse>>> GetDayWiseCollectedAmountAsync(DayWiseRequest request)
        {
            var collectedAmounts = await _feesDashboardRepository.GetDayWiseCollectedAmountAsync(request);
            return new ServiceResponse<List<DayWiseResponse>>(true, "Data retrieved successfully.", collectedAmounts, 200);
        }
        public async Task<List<FeeCollectionAnalysisResponse>> GetFeeCollectionAnalysisAsync(FeeCollectionAnalysisRequest request)
        {
            return await _feesDashboardRepository.GetFeeCollectionAnalysisAsync(request);
        }
    }
}
