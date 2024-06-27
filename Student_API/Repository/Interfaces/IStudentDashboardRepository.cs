using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Repository.Interfaces
{
    public interface IStudentDashboardRepository
    {
        Task<ServiceResponse<StudentStatisticsDTO>> GetStudentStatisticsAsync();
        Task<ServiceResponse<List<HouseWiseStudentCountDTO>>> GetHouseWiseStudentCountAsync();
        Task<ServiceResponse<List<StudentBirthdayDTO>>> GetTodaysBirthdaysAsync();
        Task<ServiceResponse<List<ClassWiseGenderCountDTO>>> GetClassWiseGenderCountAsync();
    }
}
    