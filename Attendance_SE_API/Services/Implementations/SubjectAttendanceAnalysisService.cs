using System.Formats.Asn1;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.Repository.Implementations;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using Attendance_SE_API.Services.Interfaces;
using CsvHelper;
using OfficeOpenXml;

namespace Attendance_SE_API.Services.Implementations
{
    public class SubjectAttendanceAnalysisService : ISubjectAttendanceAnalysisService
    {
        private readonly ISubjectAttendanceAnalysisRepository _repository;

        public SubjectAttendanceAnalysisService(ISubjectAttendanceAnalysisRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<SubjectAttendanceStatisticsResponse>> GetStudentAttendanceStatisticsForSubject(SubjectAttendanceAnalysisRequest request)
        {
            var response = await _repository.GetStudentAttendanceStatisticsForSubject(request);
            return new ServiceResponse<SubjectAttendanceStatisticsResponse>(
                success: true,
                message: "Student attendance statistics for the subject fetched successfully.",
                data: response,
                statusCode: 200
            );
        }
        public async Task<ServiceResponse<IEnumerable<MonthlyAttendanceSubjectAnalysisResponse>>> GetMonthlyAttendanceAnalysisForSubject(SubjectAttendanceAnalysisRequest request)
        {
            var response = await _repository.GetMonthlyAttendanceAnalysisForSubject(request);
            return response;
        }

        public async Task<ServiceResponse<IEnumerable<SubjectsAttendanceAnalysisResponse>>> GetSubjectsAttendanceAnalysis(SubjectAttendanceAnalysisRequest1 request)
        {
            var response = await _repository.GetSubjectsAttendanceAnalysis(request);
            return response;
        }
         

    }
}
