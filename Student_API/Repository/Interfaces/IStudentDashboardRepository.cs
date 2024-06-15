using Student_API.DTOs;

namespace Student_API.Repository.Interfaces
{
    public interface IStudentDashboardRepository
    {
        Task<StudentStatisticsDTO> GetStudentStatisticsAsync();
    }
}
    