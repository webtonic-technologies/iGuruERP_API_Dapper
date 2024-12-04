using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;

namespace Attendance_SE_API.Services.Interfaces
{
    public interface ISubjectAttendanceAnalysisService
    {
        Task<ServiceResponse<SubjectAttendanceStatisticsResponse>> GetStudentAttendanceStatisticsForSubject(SubjectAttendanceAnalysisRequest request);
        Task<ServiceResponse<IEnumerable<MonthlyAttendanceSubjectAnalysisResponse>>> GetMonthlyAttendanceAnalysisForSubject(SubjectAttendanceAnalysisRequest request);
        Task<ServiceResponse<IEnumerable<SubjectsAttendanceAnalysisResponse>>> GetSubjectsAttendanceAnalysis(SubjectAttendanceAnalysisRequest1 request); 
    }
}
