using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;

namespace StudentManagement_API.Services.Interfaces
{
    public interface IStudentPromotionService
    {
        Task<ServiceResponse<IEnumerable<GetClassPromotionResponse>>> GetClassPromotionAsync(GetClassPromotionRequest request);
        Task<ServiceResponse<bool>> UpdateClassPromotionConfigurationAsync(UpdateClassPromotionConfigurationRequest request);
        Task<ServiceResponse<IEnumerable<GetClassPromotionHistoryResponse>>> GetClassPromotionHistoryAsync(GetClassPromotionHistoryRequest request);
        Task<ServiceResponse<string>> GetClassPromotionHistoryExportAsync(GetClassPromotionHistoryExportRequest request);
        Task<ServiceResponse<bool>> PromoteStudentsAsync(PromoteStudentsRequest request);
        Task<ServiceResponse<IEnumerable<GetClassesResponse>>> GetClassesAsync(GetClassesRequest request);
        Task<ServiceResponse<IEnumerable<GetSectionsResponse>>> GetSectionsAsync(GetSectionsRequest request);
        Task<ServiceResponse<IEnumerable<GetStudentsPromotionResponse>>> GetStudentsAsync(GetStudentsPromotionRequest request);

    }
}
