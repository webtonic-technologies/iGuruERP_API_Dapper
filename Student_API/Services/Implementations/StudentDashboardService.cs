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

        public async Task<ServiceResponse<StudentStatisticsDTO>> GetStudentStatisticsAsync(int Institute_id)
        {
            try
            {
                var data = await _studentRepository.GetStudentStatisticsAsync(Institute_id);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<StudentStatisticsDTO>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<HouseWiseStudentCountDTO>>> GetHouseWiseStudentCountAsync(int Institute_id)
        {
            try
            {
                var data = await _studentRepository.GetHouseWiseStudentCountAsync(Institute_id);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HouseWiseStudentCountDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<StudentBirthdayDTO>>> GetTodaysBirthdaysAsync(int Institute_id)
        {
            try
            {
                var data = await _studentRepository.GetTodaysBirthdaysAsync(Institute_id);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentBirthdayDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<ClassWiseGenderCountDTO>>> GetClassWiseGenderCountAsync(int Institute_id)
        {
            try
            {
                var data = await _studentRepository.GetClassWiseGenderCountAsync(Institute_id);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ClassWiseGenderCountDTO>>(false, ex.Message, null, 500);
            }
        }
    }
}
