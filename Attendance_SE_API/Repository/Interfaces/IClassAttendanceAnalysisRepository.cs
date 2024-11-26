using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;


namespace Attendance_SE_API.Repository.Interfaces
{
    public interface IClassAttendanceAnalysisRepository
    {
        Task<AttendanceStatisticsResponse> GetStudentAttendanceStatistics(ClassAttendanceAnalysisRequest request);
        Task<ServiceResponse<IEnumerable<MonthlyAttendanceAnalysisResponse>>> GetMonthlyAttendanceAnalysis(ClassAttendanceAnalysisRequest request);
        Task<ServiceResponse<IEnumerable<AttendanceRangeAnalysisResponse>>> GetAttendanceRangeAnalysis(ClassAttendanceAnalysisRequest request);
        Task<ServiceResponse<IEnumerable<StudentDayWiseAttendanceResponse>>> GetStudentDayWiseAttendance(ClassAttendanceAnalysisRequest request);
        Task<ServiceResponse<IEnumerable<StudentAttendanceAnalysisResponse>>> GetStudentsAttendanceAnalysis(ClassAttendanceAnalysisRequest request);

    }
}
