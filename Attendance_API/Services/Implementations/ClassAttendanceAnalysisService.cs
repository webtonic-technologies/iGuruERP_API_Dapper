using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;

namespace Attendance_API.Services.Implementations
{
    public class ClassAttendanceAnalysisService : IClassAttendanceAnalysisService
    {
        private readonly IClassAttendanceAnalysisRepo _classAttendanceAnalysisRepo;
        public ClassAttendanceAnalysisService(IClassAttendanceAnalysisRepo classAttendanceAnalysisRepo)
        {
            _classAttendanceAnalysisRepo = classAttendanceAnalysisRepo;
        }
        public async Task<ServiceResponse<AttendanceStatisticsDTO>> GetAttendanceStatistics(int academicYearId, int classId, int sectionId, int instituteId)
        {
            return await _classAttendanceAnalysisRepo.GetAttendanceStatistics(academicYearId, classId, sectionId, instituteId);
        }
        
        public async Task<ServiceResponse<List<StudentAttendanceAnalysisDTO>>> GetStudentAttendanceAnalysis(int academicYearId, int classId, int sectionId, int instituteId)
        {
            return await _classAttendanceAnalysisRepo.GetStudentAttendanceAnalysis(academicYearId, classId, sectionId, instituteId);
        }
        
        public async Task<ServiceResponse<List<StudentDayWiseAttendanceDTO>>> GetStudentDayWiseAttendanceAnalysis(int academicYearId, int classId, int sectionId, int instituteId)
        {
            return await _classAttendanceAnalysisRepo.GetStudentDayWiseAttendanceAnalysis(academicYearId, classId, sectionId, instituteId);
        }
        
        public async Task<ServiceResponse<List<MonthlyAttendanceAnalysisDTO>>> GetMonthlyAttendanceAnalysis(int academicYearId, int classId, int sectionId, int instituteId)
        {
            return await _classAttendanceAnalysisRepo.GetMonthlyAttendanceAnalysis(academicYearId, classId, sectionId, instituteId);
        }
        
        public async Task<ServiceResponse<List<AttendanceRangeDTO>>> GetAttendanceRangeAnalysis(int academicYearId, int classId, int sectionId, int instituteId)
        {
            return await _classAttendanceAnalysisRepo.GetAttendanceRangeAnalysis(academicYearId, classId, sectionId, instituteId);
        }
    }
}
