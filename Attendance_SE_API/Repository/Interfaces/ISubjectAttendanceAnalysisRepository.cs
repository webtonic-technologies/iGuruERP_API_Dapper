using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;


namespace Attendance_SE_API.Repository.Interfaces
{
    public interface ISubjectAttendanceAnalysisRepository
    {
        Task<SubjectAttendanceStatisticsResponse> GetStudentAttendanceStatisticsForSubject(SubjectAttendanceAnalysisRequest request);
        Task<ServiceResponse<IEnumerable<MonthlyAttendanceSubjectAnalysisResponse>>> GetMonthlyAttendanceAnalysisForSubject(SubjectAttendanceAnalysisRequest request);
        Task<ServiceResponse<IEnumerable<SubjectsAttendanceAnalysisResponse>>> GetSubjectsAttendanceAnalysis(SubjectAttendanceAnalysisRequest1 request);

    }
}
