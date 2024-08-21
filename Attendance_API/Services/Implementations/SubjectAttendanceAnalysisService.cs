using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;

namespace Attendance_API.Services.Implementations
{
    public class SubjectAttendanceAnalysisService : ISubjectAttendanceAnalysisService
    {
        private readonly ISubjectAttendanceAnalysisRepo _subjectAttendanceAnalysisRepo;

        public SubjectAttendanceAnalysisService(ISubjectAttendanceAnalysisRepo subjectAttendanceAnalysisRepo)
        {
            _subjectAttendanceAnalysisRepo = subjectAttendanceAnalysisRepo;
        }

        public async Task<ServiceResponse<AttendanceStatisticsDTO>> GetAttendanceStatistics(int academicYearId, int classId, int sectionId, int subjectId, int instituteId)
        {
            return await _subjectAttendanceAnalysisRepo.GetAttendanceStatistics(academicYearId, classId, sectionId, subjectId, instituteId);
        }

        public async Task<ServiceResponse<List<MonthlyAttendanceAnalysisDTO>>> GetMonthlyAttendanceAnalysis(int academicYearId, int classId, int sectionId, int subjectId, int instituteId)
        {
            return await _subjectAttendanceAnalysisRepo.GetMonthlyAttendanceAnalysis(academicYearId, classId, sectionId, subjectId, instituteId);
        }

        public async Task<ServiceResponse<List<AttendanceRangeDTO>>> GetAttendanceRangeAnalysis(int academicYearId, int classId, int sectionId, int subjectId, int instituteId)
        {
            return await _subjectAttendanceAnalysisRepo.GetAttendanceRangeAnalysis(academicYearId, classId, sectionId, subjectId, instituteId);
        }

        public async Task<ServiceResponse<List<StudentAttendanceAnalysisDTO>>> GetStudentAttendanceAnalysis(int academicYearId, int classId, int sectionId, int subjectId, int instituteId)
        {
            return await _subjectAttendanceAnalysisRepo.GetStudentAttendanceAnalysis(academicYearId, classId, sectionId, subjectId, instituteId);
        }

        public async Task<ServiceResponse<List<StudentDayWiseAttendanceDTO>>> GetStudentDayWiseAttendanceAnalysis(int academicYearId, int classId, int sectionId, int subjectId, int instituteId)
        {
            return await _subjectAttendanceAnalysisRepo.GetStudentDayWiseAttendanceAnalysis(academicYearId, classId, sectionId, subjectId, instituteId);
        }
    }
}
