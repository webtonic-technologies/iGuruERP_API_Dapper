using Communication_API.DTOs.Requests.NoticeBoard;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.NoticeBoard;
using Communication_API.Repository.Interfaces.NoticeBoard;
using Communication_API.Services.Interfaces.NoticeBoard;

namespace Communication_API.Services.Implementations.NoticeBoard
{
    public class NoticeBoardService : INoticeBoardService
    {
        private readonly INoticeBoardRepository _noticeBoardRepository;

        public NoticeBoardService(INoticeBoardRepository noticeBoardRepository)
        {
            _noticeBoardRepository = noticeBoardRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateNotice(AddUpdateNoticeRequest request)
        {
            return await _noticeBoardRepository.AddUpdateNotice(request);
        }

        public async Task<ServiceResponse<List<Notice>>> GetAllNotice(GetAllNoticeRequest request)
        {
            return await _noticeBoardRepository.GetAllNotice(request);
        }

        public async Task<ServiceResponse<string>> AddUpdateCircular(AddUpdateCircularRequest request)
        {
            return await _noticeBoardRepository.AddUpdateCircular(request);
        }

        public async Task<ServiceResponse<List<Circular>>> GetAllCircular(GetAllCircularRequest request)
        {
            return await _noticeBoardRepository.GetAllCircular(request);
        }
    }
}
