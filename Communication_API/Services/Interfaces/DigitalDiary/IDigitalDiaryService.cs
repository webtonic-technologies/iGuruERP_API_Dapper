using Communication_API.DTOs.Requests.DigitalDiary;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.DigitalDiary;

namespace Communication_API.Services.Interfaces.DigitalDiary
{
    public interface IDigitalDiaryService
    {
        Task<ServiceResponse<string>> AddUpdateDiary(AddUpdateDiaryRequest request);
        Task<ServiceResponse<List<Communication_API.Models.DigitalDiary.DigitalDiary>>> GetAllDiary(GetAllDiaryRequest request);
        Task<ServiceResponse<string>> DeleteDiary(int DiaryID);
    }
}
