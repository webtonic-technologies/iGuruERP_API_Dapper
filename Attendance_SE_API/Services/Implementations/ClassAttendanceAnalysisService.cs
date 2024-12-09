using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.Services.Interfaces;
using Attendance_SE_API.ServiceResponse;
using OfficeOpenXml;

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

        //public async Task<ServiceResponse<IEnumerable<StudentAttendanceAnalysisResponse>>> GetStudentsAttendanceAnalysis(ClassAttendanceAnalysisRequest request)
        //{
        //    var response = await _repository.GetStudentsAttendanceAnalysis(request);
        //    return response;
        //}

        public async Task<ServiceResponse<IEnumerable<StudentAttendanceAnalysisResponse>>> GetStudentsAttendanceAnalysis(ClassAttendanceAnalysisRequest request)
        {
            var response = await _repository.GetStudentsAttendanceAnalysis(request);
            return response;
        }



        public async Task<byte[]> GetStudentsAttendanceAnalysisExcelExport(GetStudentsAttendanceAnalysisExcelExportRequest request)
        {
            var attendanceData = await _repository.GetStudentsAttendanceAnalysisForExport(request);
            if (attendanceData == null || !attendanceData.Any())
                return null;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Class Attendance Analysis");

            // Header
            worksheet.Cells[1, 1].Value = "S.No";
            worksheet.Cells[1, 2].Value = "Admission Number";
            worksheet.Cells[1, 3].Value = "Student Name";
            worksheet.Cells[1, 4].Value = "Total Attended (Total/Present)";
            worksheet.Cells[1, 5].Value = "Percentage";

            // Styling Header
            worksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 1, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            // Freeze header row
            worksheet.View.FreezePanes(2, 1);

            // Populate Data
            int row = 2;
            foreach (var data in attendanceData)
            {
                worksheet.Cells[row, 1].Value = row - 1; // S.No
                worksheet.Cells[row, 2].Value = data.AdmissionNumber; // Admission Number
                worksheet.Cells[row, 3].Value = data.StudentName; // Student Name
                worksheet.Cells[row, 4].Value = $"{data.TotalAttendance}/{data.TotalAttended}"; // Combined Total Attended (Total/Present)
                worksheet.Cells[row, 5].Value = data.AttendancePercentage; // Percentage
                row++;
            }

            worksheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }

    }
}
