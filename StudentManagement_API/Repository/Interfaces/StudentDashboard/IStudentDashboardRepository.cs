using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using System.Threading.Tasks;

namespace StudentManagement_API.Repository.Interfaces
{
    public interface IStudentDashboardRepository
    {
        Task<GetStudentStatisticsResponse> GetStudentStatisticsAsync(GetStudentStatisticsRequest request);
        Task<GetStudentStatusStatisticsResponse> GetStudentStatusStatisticsAsync(GetStudentStatusStatisticsRequest request);
        Task<GetStudentTypeStatisticsResponse> GetStudentTypeStatisticsAsync(GetStudentTypeStatisticsRequest request);
        Task<List<HouseWiseStudent>> GetHouseWiseStudentAsync(GetHouseWiseStudentRequest request);
        Task<List<StudentBirthday>> GetStudentBirthdaysAsync(GetStudentBirthdaysRequest request);
        Task<List<ClassWiseStudent>> GetClassWiseStudentsAsync(GetClassWiseStudentsRequest request);

    }
}
