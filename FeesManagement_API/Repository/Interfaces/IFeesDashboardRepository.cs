using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IFeesDashboardRepository
    {
        Task<decimal> GetTotalAmountCollectedAsync(int instituteId);
        Task<decimal> GetTotalPendingAmountAsync(int instituteId);
        Task<List<HeadWiseCollectedAmountResponse>> GetHeadWiseCollectedAmountAsync(HeadWiseCollectedAmountRequest request);
        Task<List<DayWiseResponse>> GetDayWiseCollectedAmountAsync(DayWiseRequest request);
        Task<List<FeeCollectionAnalysisResponse>> GetFeeCollectionAnalysisAsync(FeeCollectionAnalysisRequest request);

    }
}
