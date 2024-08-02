using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Services.Interfaces
{
    public interface IStudentDashboardService
    {
        Task<ServiceResponse<StudentStatisticsDTO>> GetStudentStatisticsAsync(int Institute_id);
        Task<ServiceResponse<List<HouseWiseStudentCountDTO>>> GetHouseWiseStudentCountAsync(int Institute_id);
        Task<ServiceResponse<List<StudentBirthdayDTO>>> GetTodaysBirthdaysAsync(int Institute_id);
        Task<ServiceResponse<List<ClassWiseGenderCountDTO>>> GetClassWiseGenderCountAsync(int Institute_id);
    }
}
