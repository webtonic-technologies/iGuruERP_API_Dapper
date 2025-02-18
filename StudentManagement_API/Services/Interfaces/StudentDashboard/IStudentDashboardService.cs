using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;
using System.Threading.Tasks;

namespace StudentManagement_API.Services.Interfaces
{
    public interface IStudentDashboardService
    {
        Task<ServiceResponse<GetStudentStatisticsResponse>> GetStudentStatisticsAsync(GetStudentStatisticsRequest request);
        Task<ServiceResponse<GetStudentStatusStatisticsResponse>> GetStudentStatusStatisticsAsync(GetStudentStatusStatisticsRequest request);
        Task<ServiceResponse<GetStudentTypeStatisticsResponse>> GetStudentTypeStatisticsAsync(GetStudentTypeStatisticsRequest request);
        Task<ServiceResponse<GetHouseWiseStudentResponse>> GetHouseWiseStudentAsync(GetHouseWiseStudentRequest request);
        Task<ServiceResponse<GetStudentBirthdaysResponse>> GetStudentBirthdaysAsync(GetStudentBirthdaysRequest request);
        Task<ServiceResponse<GetClassWiseStudentsResponse>> GetClassWiseStudentsAsync(GetClassWiseStudentsRequest request);

    }
}
