using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Services.Interfaces
{
    public interface ISubjectAttendanceAnalysisService
    {
        Task<ServiceResponse<AttendanceStatisticsDTO>> GetAttendanceStatistics(int academicYearId, int classId, int sectionId, int subjectId, int instituteId);
        Task<ServiceResponse<List<MonthlyAttendanceAnalysisDTO>>> GetMonthlyAttendanceAnalysis(int academicYearId, int classId, int sectionId, int subjectId, int instituteId);
        Task<ServiceResponse<List<AttendanceRangeDTO>>> GetAttendanceRangeAnalysis(int academicYearId, int classId, int sectionId, int subjectId, int instituteId);
        Task<ServiceResponse<List<StudentAttendanceAnalysisDTO>>> GetStudentAttendanceAnalysis(int academicYearId, int classId, int sectionId, int subjectId, int instituteId);
        Task<ServiceResponse<List<StudentDayWiseAttendanceDTO>>> GetStudentDayWiseAttendanceAnalysis(int academicYearId, int classId, int sectionId, int subjectId, int instituteId);
    }
}
