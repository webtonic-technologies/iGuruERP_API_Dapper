using Communication_API.DTOs.Requests.NoticeBoard;
using Communication_API.DTOs.Responses.NoticeBoard;
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

        public async Task<ServiceResponse<List<NoticeResponse>>> GetAllNotice(GetAllNoticeRequest request)
        {
            return await _noticeBoardRepository.GetAllNotice(request);
        }


        public async Task<ServiceResponse<string>> AddUpdateCircular(AddUpdateCircularRequest request)
        {
            return await _noticeBoardRepository.AddUpdateCircular(request);
        }

        public async Task<ServiceResponse<List<CircularResponse>>> GetAllCircular(GetAllCircularRequest request)
        {
            return await _noticeBoardRepository.GetAllCircular(request);
        }

        public async Task<ServiceResponse<string>> NoticeSetStudentView(NoticeSetStudentViewRequest request)
        {
            return await _noticeBoardRepository.NoticeSetStudentView(request);
        }
        public async Task<ServiceResponse<string>> NoticeSetEmployeeView(NoticeSetEmployeeViewRequest request)
        {
            return await _noticeBoardRepository.NoticeSetEmployeeView(request);
        }

        public async Task<ServiceResponse<StudentNoticeStatisticsResponse>> GetStudentNoticeStatistics(GetStudentNoticeStatisticsRequest request)
        {
            return await _noticeBoardRepository.GetStudentNoticeStatistics(request);
        }

        public async Task<ServiceResponse<EmployeeNoticeStatisticsResponse>> GetEmployeeNoticeStatistics(GetEmployeeNoticeStatisticsRequest request)
        {
            return await _noticeBoardRepository.GetEmployeeNoticeStatistics(request);
        }
        public async Task<ServiceResponse<string>> DeleteNotice(int InstituteID, int NoticeID)
        {
            return await _noticeBoardRepository.DeleteNotice(InstituteID, NoticeID);
        }
        public async Task<ServiceResponse<string>> DeleteCircular(DeleteCircularRequest request)
        {
            return await _noticeBoardRepository.DeleteCircular(request);
        }
        public async Task<ServiceResponse<string>> CircularSetStudentView(CircularSetStudentViewRequest request)
        {
            return await _noticeBoardRepository.CircularSetStudentView(request);
        }
        public async Task<ServiceResponse<string>> CircularSetEmployeeView(CircularSetEmployeeViewRequest request)
        {
            return await _noticeBoardRepository.CircularSetEmployeeView(request);
        }
        public async Task<ServiceResponse<StudentCircularStatisticsResponse>> GetStudentCircularStatistics(GetStudentCircularStatisticsRequest request)
        {
            return await _noticeBoardRepository.GetStudentCircularStatistics(request);
        }
        public async Task<ServiceResponse<EmployeeCircularStatisticsResponse>> GetEmployeeCircularStatistics(GetEmployeeCircularStatisticsRequest request)
        {
            return await _noticeBoardRepository.GetEmployeeCircularStatistics(request);
        }
    }
}
