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

        public async Task<IEnumerable<dynamic>> GetExportableSubjectsAttendanceData(GetSubjectsAttendanceAnalysisExportRequest request)
        {
            return await _repository.GetExportableSubjectsAttendanceData(request);
        }

        public async Task<byte[]> GetSubjectsAttendanceAnalysisExcelExport(GetSubjectsAttendanceAnalysisExportRequest request)
        {
            var attendanceData = await _repository.GetExportableSubjectsAttendanceData(request);
            if (attendanceData == null || !attendanceData.Any())
                return null;

            // Extract the list of unique subjects dynamically from flat data
            var subjects = attendanceData
                .Select(data => (string)data.SubjectName) // Extract SubjectName
                .Distinct()
                .ToList();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Subject Attendance Analysis");

            // Static headers
            worksheet.Cells[1, 1].Value = "Sr. No.";
            worksheet.Cells[1, 2].Value = "Admission No.";
            worksheet.Cells[1, 3].Value = "Student Name";

            // Dynamic headers for subjects
            int colStart = 4;
            foreach (var subject in subjects)
            {
                // Subject header (merged cells for Total Attended and Percentage)
                worksheet.Cells[1, colStart, 1, colStart + 1].Merge = true;
                worksheet.Cells[1, colStart].Value = subject;

                // Sub-headers
                worksheet.Cells[2, colStart].Value = "Total Attended (Total/Present)";
                worksheet.Cells[2, colStart + 1].Value = "Percentage";

                // Move to the next subject (2 columns per subject)
                colStart += 2;
            }

            // Styling headers
            worksheet.Cells[1, 1, 2, colStart - 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 2, colStart - 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            // Freeze the first two rows
            worksheet.View.FreezePanes(3, 1);

            // Populate data rows
            var groupedData = attendanceData
                .GroupBy(data => new { data.StudentID, data.StudentName, data.AdmissionNumber })
                .ToList();

            int row = 3;
            int srNo = 1;
            foreach (var studentGroup in groupedData)
            {
                var student = studentGroup.Key;

                worksheet.Cells[row, 1].Value = srNo++; // Sr. No.
                worksheet.Cells[row, 2].Value = student.AdmissionNumber; // Admission No.
                worksheet.Cells[row, 3].Value = student.StudentName; // Student Name

                // Dynamic subject data
                int subjectColStart = 4;
                foreach (var subject in subjects)
                {
                    var subjectData = studentGroup
                        .FirstOrDefault(data => data.SubjectName == subject);

                    if (subjectData != null)
                    {
                        worksheet.Cells[row, subjectColStart].Value = $"{subjectData.TotalAttendance}/{subjectData.TotalAttended}"; // Total Attended
                        worksheet.Cells[row, subjectColStart + 1].Value = subjectData.AttendancePercentage; // Percentage
                    }
                    else
                    {
                        worksheet.Cells[row, subjectColStart].Value = "-"; // No data for this subject
                        worksheet.Cells[row, subjectColStart + 1].Value = "-";
                    }

                    subjectColStart += 2;
                }

                row++;
            }

            worksheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }

    }
}
