using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;

namespace StudentManagement_API.Repository.Interfaces
{
    public interface IStudentPromotionRepository
    {
        Task<IEnumerable<GetClassPromotionResponse>> GetClassPromotionAsync(GetClassPromotionRequest request);
        Task<bool> UpdateClassPromotionConfigurationAsync(UpdateClassPromotionConfigurationRequest request);
        Task<(IEnumerable<GetClassPromotionHistoryResponse> Data, int TotalCount)> GetClassPromotionHistoryAsync(GetClassPromotionHistoryRequest request);
        Task<IEnumerable<GetClassPromotionHistoryExportResponse>> GetClassPromotionHistoryExportAsync(GetClassPromotionHistoryExportRequest request);
        Task<bool> PromoteStudentsAsync(PromoteStudentsRequest request);
        Task<IEnumerable<GetClassesResponse>> GetClassesAsync(GetClassesRequest request);
        Task<IEnumerable<GetSectionsResponse>> GetSectionsAsync(GetSectionsRequest request);
        Task<IEnumerable<GetStudentsPromotionResponse>> GetStudentsAsync(GetStudentsPromotionRequest request);

    }
}
