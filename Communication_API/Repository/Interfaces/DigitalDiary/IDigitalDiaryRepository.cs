using Communication_API.DTOs.Requests.DigitalDiary;
using Communication_API.DTOs.Responses;
using Communication_API.DTOs.Responses.DigitalDiary;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.DigitalDiary;

namespace Communication_API.Repository.Interfaces.DigitalDiary
{
    public interface IDigitalDiaryRepository
    {
        Task<ServiceResponse<string>> AddUpdateDiary(AddUpdateDiaryRequest request);
        Task<ServiceResponse<List<DiaryResponse>>> GetAllDiary(GetAllDiaryRequest request);
        Task<ServiceResponse<string>> DeleteDiary(int DiaryID);
        Task<IEnumerable<GetAllDiaryExportResponse>> GetAllDiaryExport(GetAllDiaryExportRequest request);

    }
}
