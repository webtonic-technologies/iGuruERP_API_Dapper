using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;
using StudentManagement_API.Repository.Interfaces;
using StudentManagement_API.Services.Interfaces;
using System.Threading.Tasks;

namespace StudentManagement_API.Services.Implementations
{
    public class StudentDashboardService : IStudentDashboardService
    {
        private readonly IStudentDashboardRepository _studentDashboardRepository;
        public StudentDashboardService(IStudentDashboardRepository studentDashboardRepository)
        {
            _studentDashboardRepository = studentDashboardRepository;
        }

        public async Task<ServiceResponse<GetStudentStatisticsResponse>> GetStudentStatisticsAsync(GetStudentStatisticsRequest request)
        {
            var data = await _studentDashboardRepository.GetStudentStatisticsAsync(request);
            return new ServiceResponse<GetStudentStatisticsResponse>(true, "Statistics retrieved successfully", data, 200);
        }

        public async Task<ServiceResponse<GetStudentStatusStatisticsResponse>> GetStudentStatusStatisticsAsync(GetStudentStatusStatisticsRequest request)
        {
            var data = await _studentDashboardRepository.GetStudentStatusStatisticsAsync(request);
            return new ServiceResponse<GetStudentStatusStatisticsResponse>(true, "Student status statistics retrieved successfully", data, 200);
        }

        public async Task<ServiceResponse<GetStudentTypeStatisticsResponse>> GetStudentTypeStatisticsAsync(GetStudentTypeStatisticsRequest request)
        {
            var data = await _studentDashboardRepository.GetStudentTypeStatisticsAsync(request);
            return new ServiceResponse<GetStudentTypeStatisticsResponse>(true, "Student type statistics retrieved successfully", data, 200);
        }

        public async Task<ServiceResponse<GetHouseWiseStudentResponse>> GetHouseWiseStudentAsync(GetHouseWiseStudentRequest request)
        {
            var houseData = await _studentDashboardRepository.GetHouseWiseStudentAsync(request);
            var response = new GetHouseWiseStudentResponse
            {
                StudentHouses = houseData
            };

            return new ServiceResponse<GetHouseWiseStudentResponse>(
                true,
                "House-wise student statistics retrieved successfully",
                response,
                200
            );
        }

        public async Task<ServiceResponse<GetStudentBirthdaysResponse>> GetStudentBirthdaysAsync(GetStudentBirthdaysRequest request)
        {
            var students = await _studentDashboardRepository.GetStudentBirthdaysAsync(request);
            var response = new GetStudentBirthdaysResponse
            {
                Students = students
            };

            return new ServiceResponse<GetStudentBirthdaysResponse>(true, "Student birthdays retrieved successfully", response, 200);
        }

        public async Task<ServiceResponse<GetClassWiseStudentsResponse>> GetClassWiseStudentsAsync(GetClassWiseStudentsRequest request)
        {
            var classWiseData = await _studentDashboardRepository.GetClassWiseStudentsAsync(request);
            var response = new GetClassWiseStudentsResponse
            {
                Students = classWiseData
            };

            return new ServiceResponse<GetClassWiseStudentsResponse>(
                true,
                "Class-wise student statistics retrieved successfully",
                response,
                200
            );
        }
    }
}
