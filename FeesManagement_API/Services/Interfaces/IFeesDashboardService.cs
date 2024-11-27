using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IFeesDashboardService
    {
        Task<TotalAmountCollectedResponse> GetTotalAmountCollectedAsync(TotalAmountCollectedRequest request);
        Task<ServiceResponse<TotalPendingAmountResponse>> GetTotalPendingAmountAsync(TotalPendingAmountRequest request);
        Task<ServiceResponse<List<HeadWiseCollectedAmountResponse>>> GetHeadWiseCollectedAmountAsync(HeadWiseCollectedAmountRequest request);
        Task<ServiceResponse<List<DayWiseResponse>>> GetDayWiseCollectedAmountAsync(DayWiseRequest request);
        Task<List<FeeCollectionAnalysisResponse>> GetFeeCollectionAnalysisAsync(FeeCollectionAnalysisRequest request);

    }
}
