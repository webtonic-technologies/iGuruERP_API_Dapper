using Attendance_SE_API.Services.Interfaces;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;
using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Repository.Implementations;
using OfficeOpenXml;
using CsvHelper;
using System.Globalization;
using System.Text;
using Attendance_SE_API.DTOs.Requests;

namespace Attendance_SE_API.Services.Implementations
{
    public class AttendanceDashboardService : IAttendanceDashboardService
    {
        private readonly IAttendanceDashboardRepository _repository;

        public AttendanceDashboardService(IAttendanceDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<DashboardAttendanceStatisticsResponse>> GetStudentAttendanceStatistics(int instituteId, string AcademicYearCode)
        {
            return await _repository.GetStudentAttendanceStatistics(instituteId, AcademicYearCode);
        }
        public async Task<ServiceResponse<List<GetStudentAttendanceDashboardResponse>>> GetStudentAttendanceDashboard(int instituteId, string AcademicYearCode, string startDate, string endDate)
        {
            return await _repository.GetStudentAttendanceDashboard(instituteId, AcademicYearCode, startDate, endDate);
        }
        public async Task<ServiceResponse<GetEmployeeAttendanceStatisticsResponse>> GetEmployeeAttendanceStatistics(int instituteId)
        {
            return await _repository.GetEmployeeAttendanceStatistics(instituteId);
        }
        public async Task<ServiceResponse<List<GetEmployeeOnLeaveResponse>>> GetEmployeeOnLeave(int instituteId)
        {
            return await _repository.GetEmployeeOnLeave(instituteId);
        }

        public async Task<ServiceResponse<List<GetAttendanceNotMarkedResponse>>> GetAttendanceNotMarked(int instituteId)
        {
            var data = await _repository.GetAttendanceNotMarked(instituteId);

            if (data != null)
            {
                return new ServiceResponse<List<GetAttendanceNotMarkedResponse>>(true, "Data retrieved successfully", data, 200);
            }

            return new ServiceResponse<List<GetAttendanceNotMarkedResponse>>(false, "No data found", null, 404);
        }

        public async Task<ServiceResponse<List<GetAbsentStudentsResponse>>> GetAbsentStudents(int instituteId)
        {
            var data = await _repository.GetAbsentStudents(instituteId);

            if (data != null)
            {
                return new ServiceResponse<List<GetAbsentStudentsResponse>>(true, "Absent students data fetched successfully.", data, 200);
            }

            return new ServiceResponse<List<GetAbsentStudentsResponse>>(false, "No data found for absent students.", null, 404);
        }

        public async Task<ServiceResponse<GetStudentsMLCountResponse>> GetStudentsMLCount(int instituteId)
        {
            var data = await _repository.GetStudentsMLCount(instituteId);

            if (data != null)
            {
                return new ServiceResponse<GetStudentsMLCountResponse>(true, "Medical leave count fetched successfully.", data, 200);
            }

            return new ServiceResponse<GetStudentsMLCountResponse>(false, "No data found for medical leave count.", null, 404);
        }

        public async Task<ServiceResponse<GetHalfDayLeaveCountResponse>> GetHalfDayLeaveCount(int instituteId)
        {
            var data = await _repository.GetHalfDayLeaveCount(instituteId);

            if (data != null)
            {
                return new ServiceResponse<GetHalfDayLeaveCountResponse>(true, "Half-day leave count fetched successfully.", data, 200);
            }

            return new ServiceResponse<GetHalfDayLeaveCountResponse>(false, "No data found for half-day leave count.", null, 404);
        }

        public async Task<ServiceResponse<byte[]>> GetAttendanceNotMarkedExport(GetAttendanceNotMarkedExportRequest request)
        {
            try
            {
                var data = await _repository.GetAttendanceNotMarkedExport(request);

                byte[] exportData = null;

                if (request.ExportType == 1) // Excel export
                {
                    exportData = GenerateExcel(data);
                }
                else if (request.ExportType == 2) // CSV export
                {
                    exportData = GenerateCsv(data);
                }

                return new ServiceResponse<byte[]>(
                    true,
                    "Export completed successfully",
                    exportData,
                    200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }

        private byte[] GenerateExcel(IEnumerable<GetAttendanceNotMarkedExportResponse> data)
        {
            using (var memoryStream = new MemoryStream())
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("AttendanceNotMarked");
                worksheet.Cells["A1"].Value = "Class";
                worksheet.Cells["B1"].Value = "Section";
                worksheet.Cells["C1"].Value = "Section Strength";
                worksheet.Cells["D1"].Value = "Class Teacher";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.Class;
                    worksheet.Cells[row, 2].Value = item.Section;
                    worksheet.Cells[row, 3].Value = item.SectionStrength;
                    worksheet.Cells[row, 4].Value = item.ClassTeacher;
                    row++;
                }

                package.Save();
                return memoryStream.ToArray();
            }
        }
        private byte[] GenerateCsv(IEnumerable<GetAttendanceNotMarkedExportResponse> data)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<GetAttendanceNotMarkedExportResponse>();
                csv.NextRecord();

                foreach (var item in data)
                {
                    csv.WriteRecord(item);
                    csv.NextRecord();
                }

                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }

        public async Task<ServiceResponse<byte[]>> GetAbsentStudentsExport(GetAbsentStudentsExportRequest request)
        {
            try
            {
                var absentStudents = await _repository.GetAbsentStudentsExport(request);

                byte[] exportData = null;

                if (request.ExportType == 1) // Excel export
                {
                    exportData = GenerateExcel(absentStudents);
                }
                else if (request.ExportType == 2) // CSV export
                {
                    exportData = GenerateCsv(absentStudents);
                }

                return new ServiceResponse<byte[]>(
                    true,
                    "Export completed successfully",
                    exportData,
                    200
                );
            }
            catch (System.Exception ex)
            {
                return new ServiceResponse<byte[]>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }

        private byte[] GenerateExcel(IEnumerable<GetAbsentStudentsExportResponse> absentStudents)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            using (var package = new OfficeOpenXml.ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Absent Students");
                worksheet.Cells["A1"].Value = "Student Name";
                worksheet.Cells["B1"].Value = "Class Name";
                worksheet.Cells["C1"].Value = "Section Name";
                worksheet.Cells["D1"].Value = "Admission Number";

                int row = 2;
                foreach (var student in absentStudents)
                {
                    worksheet.Cells[row, 1].Value = student.StudentName;
                    worksheet.Cells[row, 2].Value = student.ClassName;
                    worksheet.Cells[row, 3].Value = student.SectionName;
                    worksheet.Cells[row, 4].Value = student.AdmissionNumber;
                    row++;
                }

                package.Save();
                return memoryStream.ToArray();
            }
        }

        private byte[] GenerateCsv(IEnumerable<GetAbsentStudentsExportResponse> absentStudents)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            using (var streamWriter = new System.IO.StreamWriter(memoryStream, System.Text.Encoding.UTF8))
            using (var csv = new CsvHelper.CsvWriter(streamWriter, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<GetAbsentStudentsExportResponse>();
                csv.NextRecord();

                foreach (var student in absentStudents)
                {
                    csv.WriteRecord(student);
                    csv.NextRecord();
                }

                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}
