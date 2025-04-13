using System.Threading.Tasks;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;

namespace FeesManagement_API.Services.Implementations
{
    public class FeesDashboardService : IFeesDashboardService
    {
        private readonly IFeesDashboardRepository _feesDashboardRepository;

        public FeesDashboardService(IFeesDashboardRepository feesDashboardRepository)
        {
            _feesDashboardRepository = feesDashboardRepository;
        }

        public async Task<ServiceResponse<GetFeeStatisticsResponse>> GetFeeStatisticsAsync(int instituteId)
        {
            var (totalAmountCollected, totalPendingAmount, totalFineCollected) = await _feesDashboardRepository.GetFeeStatisticsAsync(instituteId);

            var response = new GetFeeStatisticsResponse
            {
                TotalAmountCollected = totalAmountCollected,
                TotalPendingAmount = totalPendingAmount,
                TotalFineCollected = totalFineCollected
            };

            return new ServiceResponse<GetFeeStatisticsResponse>(true, "Fee statistics retrieved successfully", response, 200);
        }

        public async Task<ServiceResponse<GetHeadWiseCollectedAmountResponse>> GetHeadWiseCollectedAmountAsync(int instituteId)
        {
            var result = await _feesDashboardRepository.GetHeadWiseCollectedAmountAsync(instituteId);
            return new ServiceResponse<GetHeadWiseCollectedAmountResponse>(true, "Head-wise collected amounts retrieved successfully", result, 200);
        }

        public async Task<ServiceResponse<GetDayWiseFeesResponse>> GetDayWiseFeesAsync(int instituteId)
        {
            var responseData = await _feesDashboardRepository.GetDayWiseFeesAsync(instituteId);
            return new ServiceResponse<GetDayWiseFeesResponse>(true, "Day-wise fee collection retrieved successfully", responseData, 200);
        }

        public async Task<ServiceResponse<GetClassSectionWiseResponse>> GetClassSectionWiseAsync(int instituteId)
        {
            var result = await _feesDashboardRepository.GetClassSectionWiseAsync(instituteId);
            return new ServiceResponse<GetClassSectionWiseResponse>(
                true,
                "Class-section wise fee collection retrieved successfully",
                result,
                200);
        }

        public async Task<ServiceResponse<GetTypeWiseCollectionResponse>> GetTypeWiseCollectionAsync(int instituteId)
        {
            var result = await _feesDashboardRepository.GetTypeWiseCollectionAsync(instituteId);
            return new ServiceResponse<GetTypeWiseCollectionResponse>(
                true,
                "Type-wise collection retrieved successfully",
                result,
                200);
        }

        public async Task<ServiceResponse<GetModeWiseCollectionResponse>> GetModeWiseCollectionAsync(int instituteId, int month, int year)
        {
            var result = await _feesDashboardRepository.GetModeWiseCollectionAsync(instituteId, month, year);
            return new ServiceResponse<GetModeWiseCollectionResponse>(
                true,
                "Mode-wise collection retrieved successfully",
                result,
                200);
        }

        public async Task<ServiceResponse<GetCollectionAnalysisResponse>> GetCollectionAnalysisAsync(int instituteId)
        {
            var result = await _feesDashboardRepository.GetCollectionAnalysisAsync(instituteId);
            return new ServiceResponse<GetCollectionAnalysisResponse>(
                true,
                "Collection analysis retrieved successfully",
                result,
                200);
        }
    }
}

 