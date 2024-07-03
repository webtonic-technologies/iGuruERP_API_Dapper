using Communication_API.DTOs.Requests.NoticeBoard;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.NoticeBoard;

namespace Communication_API.Services.Interfaces.NoticeBoard
{
    public interface INoticeBoardService
    {
        Task<ServiceResponse<string>> AddUpdateNotice(AddUpdateNoticeRequest request);
        Task<ServiceResponse<List<Notice>>> GetAllNotice(GetAllNoticeRequest request);
        Task<ServiceResponse<string>> AddUpdateCircular(AddUpdateCircularRequest request);
        Task<ServiceResponse<List<Circular>>> GetAllCircular(GetAllCircularRequest request);
    }
}
