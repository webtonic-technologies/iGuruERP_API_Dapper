using Communication_API.DTOs.Requests.DigitalDiary;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.DigitalDiary;
using Communication_API.Repository.Interfaces.DigitalDiary;
using Communication_API.Services.Interfaces.DigitalDiary;

namespace Communication_API.Services.Implementations.DigitalDiary
{
    public class DigitalDiaryService : IDigitalDiaryService
    {
        private readonly IDigitalDiaryRepository _digitalDiaryRepository;

        public DigitalDiaryService(IDigitalDiaryRepository digitalDiaryRepository)
        {
            _digitalDiaryRepository = digitalDiaryRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateDiary(AddUpdateDiaryRequest request)
        {
            return await _digitalDiaryRepository.AddUpdateDiary(request);
        }

        public async Task<ServiceResponse<List<Communication_API.Models.DigitalDiary.DigitalDiary>>> GetAllDiary(GetAllDiaryRequest request)
        {
            return await _digitalDiaryRepository.GetAllDiary(request);
        }

        public async Task<ServiceResponse<string>> DeleteDiary(int DiaryID)
        {
            return await _digitalDiaryRepository.DeleteDiary(DiaryID);
        }
    }
}
