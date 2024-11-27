using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.Services.Interfaces;
using Attendance_SE_API.ServiceResponse;
 
namespace Attendance_SE_API.Services.Implementations
{
    public class ClassAttendanceAnalysisService : IClassAttendanceAnalysisService
    {
        private readonly IClassAttendanceAnalysisRepository _repository;

        public ClassAttendanceAnalysisService(IClassAttendanceAnalysisRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<AttendanceStatisticsResponse>> GetStudentAttendanceStatistics(ClassAttendanceAnalysisRequest request)
        {
            // Initialize the response with default values
            ServiceResponse<AttendanceStatisticsResponse> response = null;

            try
            {
                // Fetch the attendance statistics from the repository
                var result = await _repository.GetStudentAttendanceStatistics(request);

                // Populate the response with the result
                response = new ServiceResponse<AttendanceStatisticsResponse>(
                    success: true,
                    message: "Attendance statistics fetched successfully.",
                    data: result,  // This should be the data from the repository
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                // If an error occurs, return a failed response with an error message
                response = new ServiceResponse<AttendanceStatisticsResponse>(
                    success: false,
                    message: $"An error occurred: {ex.Message}",
                    data: null,  // No data on error
                    statusCode: 500
                );
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<MonthlyAttendanceAnalysisResponse>>> GetMonthlyAttendanceAnalysis(ClassAttendanceAnalysisRequest request)
        {
            var response = await _repository.GetMonthlyAttendanceAnalysis(request);
            return response;
        }
        public async Task<ServiceResponse<IEnumerable<AttendanceRangeAnalysisResponse>>> GetAttendanceRangeAnalysis(ClassAttendanceAnalysisRequest request)
        {
            var response = await _repository.GetAttendanceRangeAnalysis(request);
            return response;
        }

        public async Task<ServiceResponse<IEnumerable<StudentDayWiseAttendanceResponse>>> GetStudentDayWiseAttendance(ClassAttendanceAnalysisRequest request)
        {
            var response = await _repository.GetStudentDayWiseAttendance(request);
            return response;
        }

        public async Task<ServiceResponse<IEnumerable<StudentAttendanceAnalysisResponse>>> GetStudentsAttendanceAnalysis(ClassAttendanceAnalysisRequest request)
        {
            var response = await _repository.GetStudentsAttendanceAnalysis(request);
            return response;
        }

    }
}
