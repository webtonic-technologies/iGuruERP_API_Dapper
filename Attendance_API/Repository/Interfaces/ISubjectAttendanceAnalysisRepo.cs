using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Repository.Interfaces
{
    public interface ISubjectAttendanceAnalysisRepo
    {
        Task<ServiceResponse<AttendanceStatisticsDTO>> GetAttendanceStatistics(int academicYearId, int classId, int sectionId, int subjectId, int instituteId);
        Task<ServiceResponse<List<MonthlyAttendanceAnalysisDTO>>> GetMonthlyAttendanceAnalysis(int academicYearId, int classId, int sectionId, int subjectId, int instituteId);
        Task<ServiceResponse<List<MonthlyAttendanceAnalysisDTO>>> GetAttendanceRangeAnalysis(int academicYearId, int classId, int sectionId, int subjectId, int instituteId);
        Task<ServiceResponse<List<StudentAttendanceAnalysisDTO>>> GetStudentAttendanceAnalysis(int academicYearId, int classId, int sectionId, int subjectId, int instituteId);
        Task<ServiceResponse<List<StudentDayWiseAttendanceDTO>>> GetStudentDayWiseAttendanceAnalysis(int academicYearId, int classId, int sectionId, int subjectId, int instituteId);
    }
}
