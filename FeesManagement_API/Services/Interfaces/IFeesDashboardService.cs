using System.Threading.Tasks;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IFeesDashboardService
    {
        Task<ServiceResponse<GetFeeStatisticsResponse>> GetFeeStatisticsAsync(int instituteId);
        Task<ServiceResponse<GetHeadWiseCollectedAmountResponse>> GetHeadWiseCollectedAmountAsync(int instituteId);
        Task<ServiceResponse<GetDayWiseFeesResponse>> GetDayWiseFeesAsync(int instituteId);
        Task<ServiceResponse<GetClassSectionWiseResponse>> GetClassSectionWiseAsync(int instituteId);
        Task<ServiceResponse<GetTypeWiseCollectionResponse>> GetTypeWiseCollectionAsync(int instituteId);
        Task<ServiceResponse<GetModeWiseCollectionResponse>> GetModeWiseCollectionAsync(int instituteId, int month, int year);
        Task<ServiceResponse<GetCollectionAnalysisResponse>> GetCollectionAnalysisAsync(int instituteId);

    }
}
 
