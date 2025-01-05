using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using Attendance_SE_API.Services.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks; 

namespace Attendance_SE_API.Services.Implementations
{
    public class EmployeeAttendanceReportService : IEmployeeAttendanceReportService
    {
        private readonly IEmployeeAttendanceReportRepository _repository;

        public EmployeeAttendanceReportService(IEmployeeAttendanceReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<EmployeeAttendanceReportResponse> GetAttendanceReport(EmployeeAttendanceReportRequest request)
        {
            return await _repository.GetAttendanceReport(request);
        }

         
        public async Task<ServiceResponse<IEnumerable<GetAttendanceGeoFencingReportResponse>>> GetAttendanceGeoFencingReport(GetAttendanceGeoFencingReportRequest request)
        {
            try
            {
                // Fetch the data from the repository
                var data = await _repository.GetAttendanceGeoFencingReport(request);

                // Return a successful response with the required parameters
                return new ServiceResponse<IEnumerable<GetAttendanceGeoFencingReportResponse>>(
                    success: true,
                    message: "Attendance GeoFencing report fetched successfully",
                    data: data,
                    statusCode: 200,
                    totalCount: data?.Count() // Optional: total count of data
                );
            }
            catch (Exception ex)
            {
                // Return a failed response in case of an error
                return new ServiceResponse<IEnumerable<GetAttendanceGeoFencingReportResponse>>(
                    success: false,
                    message: ex.Message,
                    data: null,
                    statusCode: 500,
                    totalCount: null // No data, so the count is null
                );
            }
        }


        public async Task<MemoryStream> GenerateExcelReport(GetAttendanceGeoFencingReportRequest request)
        {
            return await _repository.GenerateExcelReport(request);
        }

        public async Task<MemoryStream> GenerateCSVReport(GetAttendanceGeoFencingReportRequest request)
        {
            // Fetch the data first
            var data = await _repository.GetAttendanceGeoFencingReport(request);

            // Create a MemoryStream to write the CSV
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            var csvBuilder = new StringBuilder();

            // Write the header row
            csvBuilder.AppendLine("EmployeeID,Employee Name,Primary Mobile No.");

            // Add the date columns (dynamic based on the attendance data)
            var dateColumns = new List<string>();
            foreach (var item in data)
            {
                foreach (var attendance in item.Attendance)
                {
                    if (!dateColumns.Contains(attendance.Date))
                    {
                        dateColumns.Add(attendance.Date);
                    }
                }
            }

            // Add headers for each date (Clock In, Clock Out, Time)
            foreach (var date in dateColumns)
            {
                csvBuilder.Append($"{date} Clock In,{date} Clock Out,{date} Time,");
            }

            // Remove the last comma
            csvBuilder.Remove(csvBuilder.Length - 1, 1);
            csvBuilder.AppendLine();

            // Write the employee attendance data
            foreach (var item in data)
            {
                csvBuilder.Append($"{item.EmployeeCode},{item.EmployeeName},{item.PrimaryMobileNumber},");

                // Add attendance details for each date
                foreach (var date in dateColumns)
                {
                    var attendance = item.Attendance.FirstOrDefault(a => a.Date == date);
                    if (attendance != null)
                    {
                        csvBuilder.Append($"Clock In : {attendance.ClockIn},");
                        csvBuilder.Append($"Clock Out : {attendance.ClockOut},");
                        csvBuilder.Append($"Time : {attendance.Time},");
                    }
                    else
                    {
                        csvBuilder.Append("N/A,N/A,N/A,");
                    }
                }

                // Remove the last comma
                csvBuilder.Remove(csvBuilder.Length - 1, 1);
                csvBuilder.AppendLine();
            }

            // Write the CSV data to the MemoryStream
            writer.Write(csvBuilder.ToString());
            writer.Flush();

            // Reset stream position to the beginning
            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<List<AttendanceMode>> GetAttendanceMode()
        {
            var attendanceModes = await _repository.GetAttendanceModes();
            return attendanceModes;
        }

        public async Task<ServiceResponse<IEnumerable<GetAttendanceBioMericReportResponse>>> GetAttendanceBioMericReport(GetAttendanceBioMericReportRequest request)
        {
            try
            {
                // Fetch the data from the repository
                var data = await _repository.GetAttendanceBioMericReport(request);

                // Return a successful response with the required parameters
                return new ServiceResponse<IEnumerable<GetAttendanceBioMericReportResponse>>(
                    true,
                    "Attendance BioMetric report fetched successfully",
                    data,
                    200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetAttendanceBioMericReportResponse>>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }

        public async Task<MemoryStream> GenerateBioMetricExcelReport(GetAttendanceBioMericReportRequest request)
        {
            return await _repository.GenerateBioMetricExcelReport(request);
        }

        public async Task<MemoryStream> GenerateBioMetricCSVReport(GetAttendanceBioMericReportRequest request)
        {
            // Fetch the data first
            var data = await _repository.GetAttendanceBioMericReport(request);

            // Create a MemoryStream to write the CSV
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            var csvBuilder = new StringBuilder();

            // Write the header row
            csvBuilder.AppendLine("EmployeeID,Employee Name,Primary Mobile No.");

            // Add the date columns (dynamic based on the attendance data)
            var dateColumns = new List<string>();
            foreach (var item in data)
            {
                foreach (var attendance in item.BioMetricAttendance)
                {
                    if (!dateColumns.Contains(attendance.Date))
                    {
                        dateColumns.Add(attendance.Date);
                    }
                }
            }

            // Add headers for each date (Clock In, Clock Out, Time)
            foreach (var date in dateColumns)
            {
                csvBuilder.Append($"{date} Clock In,{date} Clock Out,{date} Time,");
            }

            // Remove the last comma
            csvBuilder.Remove(csvBuilder.Length - 1, 1);
            csvBuilder.AppendLine();

            // Write the employee attendance data
            foreach (var item in data)
            {
                csvBuilder.Append($"{item.EmployeeCode},{item.EmployeeName},{item.PrimaryMobileNumber},");

                // Add attendance details for each date
                foreach (var date in dateColumns)
                {
                    var attendance = item.BioMetricAttendance.FirstOrDefault(a => a.Date == date);
                    if (attendance != null)
                    {
                        csvBuilder.Append($"Clock In : {attendance.ClockIn},");
                        csvBuilder.Append($"Clock Out : {attendance.ClockOut},");
                        csvBuilder.Append($"Time : {attendance.Time},");
                    }
                    else
                    {
                        csvBuilder.Append("N/A,N/A,N/A,");
                    }
                }

                // Remove the last comma
                csvBuilder.Remove(csvBuilder.Length - 1, 1);
                csvBuilder.AppendLine();
            }

            // Write the CSV data to the MemoryStream
            writer.Write(csvBuilder.ToString());
            writer.Flush();

            // Reset stream position to the beginning
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
