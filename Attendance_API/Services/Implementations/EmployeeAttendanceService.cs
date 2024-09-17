using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;
using OfficeOpenXml;
using System.IO;

namespace Attendance_API.Services.Implementations
{
    public class EmployeeAttendanceService : IEmployeeAttendanceService
    {
        private readonly IEmployeeAttendanceRepository _employeeAttendanceRepository;

        public EmployeeAttendanceService(IEmployeeAttendanceRepository employeeAttendanceRepository)
        {
            _employeeAttendanceRepository = employeeAttendanceRepository;
        }

        public async Task<ServiceResponse<EmployeeAttendanceMasterResponseDTO>> GetEmployeeAttendanceMasterList(EmployeeAttendanceMasterRequestDTO request)
        {
            return await _employeeAttendanceRepository.GetEmployeeAttendanceMasterList(request);
        }

        public async Task<ServiceResponse<EmployeeAttendanceMasterDTO>> InsertOrUpdateEmployeeAttendanceMaster(EmployeeAttendanceMasterDTO employeeAttendanceMaster)
        {
            return await _employeeAttendanceRepository.InsertOrUpdateEmployeeAttendanceMaster(employeeAttendanceMaster);
        }

        public async Task<ServiceResponse<bool>> DeleteEmployeeAttendanceMaster(int employeeAttendanceId)
        {
            return await _employeeAttendanceRepository.DeleteEmployeeAttendanceMaster(employeeAttendanceId);
        }

        public async Task<ServiceResponse<dynamic>> GetEmployeeAttendanceReport(EmployeeAttendanceReportRequestDTO request)
        {
            return await _employeeAttendanceRepository.GetEmployeeAttendanceReport(request);
        }

        public async Task<ServiceResponse<string>> ExportEmployeeAttendanceReportToExcel(EmployeeAttendanceReportRequestDTO request)
        {
            var response = await _employeeAttendanceRepository.GetEmployeeAttendanceReport(request);
            if (!response.Success)
            {
                return new ServiceResponse<string>(false, "No data found for export", null, 404);
            }

            // Create an Excel package using EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employee Attendance");

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
                var fileName = $"EmployeeAttendance_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                await File.WriteAllBytesAsync(filePath, package.GetAsByteArray());

                return new ServiceResponse<string>(true, "Excel file generated successfully", filePath, 200);
            }
        }
    }
}
