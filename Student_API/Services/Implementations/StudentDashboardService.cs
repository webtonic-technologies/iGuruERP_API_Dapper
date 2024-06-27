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
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<StudentStatisticsDTO>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<HouseWiseStudentCountDTO>>> GetHouseWiseStudentCountAsync()
        {
            try
            {
                var data = await _studentRepository.GetHouseWiseStudentCountAsync();
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HouseWiseStudentCountDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<StudentBirthdayDTO>>> GetTodaysBirthdaysAsync()
        {
            try
            {
                var data = await _studentRepository.GetTodaysBirthdaysAsync();
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentBirthdayDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<ClassWiseGenderCountDTO>>> GetClassWiseGenderCountAsync()
        {
            try
            {
                var data = await _studentRepository.GetClassWiseGenderCountAsync();
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ClassWiseGenderCountDTO>>(false, ex.Message, null, 500);
            }
        }
    }
}
