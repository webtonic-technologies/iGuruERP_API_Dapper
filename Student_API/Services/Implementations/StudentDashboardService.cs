using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;

namespace Student_API.Services.Implementations
{
    public class StudentDashboardService : IStudentDashboardService
    {
        private readonly IStudentDashboardRepository _studentRepository;

        public StudentDashboardService(IStudentDashboardRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<ServiceResponse<StudentStatisticsDTO>> GetStudentStatisticsAsync()
        {
            try
            {
                var data = await _studentRepository.GetStudentStatisticsAsync();
                return new ServiceResponse<StudentStatisticsDTO>(true, "Data retrieved successfully", data, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<StudentStatisticsDTO>(false, ex.Message, null, 500);
            }
        }
    }
}
