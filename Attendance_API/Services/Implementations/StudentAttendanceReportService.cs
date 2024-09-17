using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;
using OfficeOpenXml;

namespace Attendance_API.Services.Implementations
{
    public class StudentAttendanceReportService : IStudentAttendanceReportService
    {
        private readonly IStudentAttendanceReportRepository _studentAttendanceReportRepository;
        public StudentAttendanceReportService(IStudentAttendanceReportRepository studentAttendanceReportRepository)
        {
            _studentAttendanceReportRepository = studentAttendanceReportRepository; 
        }
        
        public async Task<ServiceResponse<dynamic>> GetStudentAttendanceDatewiseReport(StudentAttendanceDatewiseReportRequestDTO request)
        {
            return await _studentAttendanceReportRepository.GetStudentAttendanceDatewiseReport(request);
        }

        public async Task<ServiceResponse<dynamic>> GetStudentSubjectwiseReport(SubjectwiseAttendanceReportRequest request)
        {
            return await _studentAttendanceReportRepository.GetStudentSubjectwiseReport(request);
        }

        public async Task<ServiceResponse<string>> ExportStudentAttendanceDatewiseReportToExcel(StudentAttendanceDatewiseReportRequestDTO request)
        {
            var response = await _studentAttendanceReportRepository.GetStudentAttendanceDatewiseReport(request);
            if (!response.Success)
            {
                return new ServiceResponse<string>(false, "No data found for export", null, 404);
            }

            // Create an Excel package using EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Datewise Attendance");

                var data = response.Data;
                if (data.Any())
                {
                    var firstRecord = data.First();
                    var properties = firstRecord.GetType().GetProperties();

                    // Set headers
                    for (int i = 0; i < properties.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = properties[i].Name;
                    }

                    // Add data rows
                    var rowIndex = 2; // Start from row 2 as row 1 contains headers
                    foreach (var record in data)
                    {
                        for (int i = 0; i < properties.Length; i++)
                        {
                            var value = properties[i].GetValue(record);
                            // Handle different types of data if needed (e.g., date formatting)
                            worksheet.Cells[rowIndex, i + 1].Value = value ?? string.Empty;
                        }
                        rowIndex++;
                    }

                    // Auto-fit columns for better readability
                    worksheet.Cells.AutoFitColumns();
                }
                else
                {
                    // Handle case where data is empty but response.Success is true
                    worksheet.Cells[1, 1].Value = "No data available";
                }


                // Save the file to a specific location
                var fileName = $"DatewiseAttendance_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                await File.WriteAllBytesAsync(filePath, package.GetAsByteArray());

                return new ServiceResponse<string>(true, "Excel file generated successfully", filePath, 200);
            }
        }

        public async Task<ServiceResponse<string>> ExportStudentSubjectwiseReportToExcel(SubjectwiseAttendanceReportRequestExport request)
        {

            SubjectwiseAttendanceReportRequest model = new();
            model.section_id= request.section_id;
            model.class_id= request.class_id;
            model.Date = request.Date;
            model.PageNumber = 1;
            model.PageSize = int.MaxValue; 
            var response = await _studentAttendanceReportRepository.GetStudentSubjectwiseReport(model);
            if (!response.Success)
            {
                return new ServiceResponse<string>(false, "No data found for export", null, 404);
            }

            // Create an Excel package using EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Subjectwise Attendance");

                var data = response.Data;
                if (data.Any())
                {
                    var firstRecord = data.First();
                    var properties = firstRecord.GetType().GetProperties();

                    // Set headers
                    for (int i = 0; i < properties.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = properties[i].Name;
                    }

                    // Add data rows
                    var rowIndex = 2; // Start from row 2 as row 1 contains headers
                    foreach (var record in data)
                    {
                        for (int i = 0; i < properties.Length; i++)
                        {
                            var value = properties[i].GetValue(record);
                            // Handle different types of data if needed (e.g., date formatting)
                            worksheet.Cells[rowIndex, i + 1].Value = value ?? string.Empty;
                        }
                        rowIndex++;
                    }

                    // Auto-fit columns for better readability
                    worksheet.Cells.AutoFitColumns();
                }
                else
                {
                    // Handle case where data is empty but response.Success is true
                    worksheet.Cells[1, 1].Value = "No data available";
                }


                // Save the file to a specific location
                var fileName = $"SubjectwiseAttendance_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                await File.WriteAllBytesAsync(filePath, package.GetAsByteArray());

                return new ServiceResponse<string>(true, "Excel file generated successfully", filePath, 200);
            }
        }
    }
}
