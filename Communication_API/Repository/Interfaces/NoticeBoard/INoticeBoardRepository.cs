using Communication_API.DTOs.Requests.NoticeBoard;
using Communication_API.DTOs.Responses.NoticeBoard;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.NoticeBoard;

namespace Communication_API.Repository.Interfaces.NoticeBoard
{
    public interface INoticeBoardRepository
    {
        Task<ServiceResponse<string>> AddUpdateNotice(AddUpdateNoticeRequest request);
        Task<ServiceResponse<List<NoticeResponse>>> GetAllNotice(GetAllNoticeRequest request);
        Task<ServiceResponse<string>> AddUpdateCircular(AddUpdateCircularRequest request);
        Task<ServiceResponse<List<CircularResponse>>> GetAllCircular(GetAllCircularRequest request); // Update this line
        Task<ServiceResponse<string>> NoticeSetStudentView(NoticeSetStudentViewRequest request);
        Task<ServiceResponse<string>> NoticeSetEmployeeView(NoticeSetEmployeeViewRequest request);
        Task<ServiceResponse<StudentNoticeStatisticsResponse>> GetStudentNoticeStatistics(GetStudentNoticeStatisticsRequest request);
        Task<ServiceResponse<EmployeeNoticeStatisticsResponse>> GetEmployeeNoticeStatistics(GetEmployeeNoticeStatisticsRequest request);
        Task<ServiceResponse<string>> DeleteNotice(int InstituteID, int NoticeID);
        Task<ServiceResponse<string>> DeleteCircular(DeleteCircularRequest request);
        Task<ServiceResponse<string>> CircularSetStudentView(CircularSetStudentViewRequest request);
        Task<ServiceResponse<string>> CircularSetEmployeeView(CircularSetEmployeeViewRequest request); 
        Task<ServiceResponse<StudentCircularStatisticsResponse>> GetStudentCircularStatistics(GetStudentCircularStatisticsRequest request);
        Task<ServiceResponse<EmployeeCircularStatisticsResponse>> GetEmployeeCircularStatistics(GetEmployeeCircularStatisticsRequest request);



    }
}
