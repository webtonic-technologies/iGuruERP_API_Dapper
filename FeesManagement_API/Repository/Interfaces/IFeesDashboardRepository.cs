using FeesManagement_API.DTOs.Responses;
using System.Threading.Tasks;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IFeesDashboardRepository
    { 
        Task<(decimal totalAmountCollected, decimal totalPendingAmount, decimal totalFineCollected)> GetFeeStatisticsAsync(int instituteId);
        Task<GetHeadWiseCollectedAmountResponse> GetHeadWiseCollectedAmountAsync(int instituteId);
        Task<GetDayWiseFeesResponse> GetDayWiseFeesAsync(int instituteId);
        Task<GetClassSectionWiseResponse> GetClassSectionWiseAsync(int instituteId);
        Task<GetTypeWiseCollectionResponse> GetTypeWiseCollectionAsync(int instituteId);
        Task<GetModeWiseCollectionResponse> GetModeWiseCollectionAsync(int instituteId, int month, int year);
        Task<GetCollectionAnalysisResponse> GetCollectionAnalysisAsync(int instituteId);


    }
} 