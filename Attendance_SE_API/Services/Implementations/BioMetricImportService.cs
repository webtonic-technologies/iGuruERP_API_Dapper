using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using System.Threading.Tasks;
using System.Data;
using System;
using System.Globalization;
using OfficeOpenXml;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.Services.Interfaces;

namespace Attendance_SE_API.Services.Implementations
{
    public class BioMetricImportService : IBioMetricImportService
    {
        private readonly IBioMetricImportRepository _bioMetricImportRepository;

        public BioMetricImportService(IBioMetricImportRepository bioMetricImportRepository)
        {
            _bioMetricImportRepository = bioMetricImportRepository;
        }

        public ServiceResponse<DataTable> DownloadBioMetricTemplate(BioMetricImportRequest request)
        {
            DataTable bioMetricData = _bioMetricImportRepository.GetBioMetricData(request);
            return new ServiceResponse<DataTable>(true, "BioMetric Template Retrieved", bioMetricData, 200);
        }

        public byte[] GenerateExcelFile(DataTable dataTable, DateTime startDate, DateTime endDate)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("BioMetric");

                // Adding fixed columns headers
                worksheet.Cells[1, 1].Value = "EmployeeID";  
                worksheet.Cells[1, 2].Value = "EmployeeName";



                // Adding day columns with Clock-In and Clock-Out under each date
                int column = 3;
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    string dateHeader = date.ToString("MMM dd, ddd");

                    // Merge cells for the date and "Clock-In"/"Clock-Out"
                    worksheet.Cells[1, column].Value = dateHeader;
                    worksheet.Cells[1, column, 1, column + 1].Merge = true;  // Merge date and the next two columns
                    worksheet.Cells[1, column, 1, column + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                    // Set "Clock-In" and "Clock-Out" for the second row under the merged date
                    worksheet.Cells[2, column].Value = "Clock-In";
                    worksheet.Cells[2, column + 1].Value = "Clock-Out";

                    // Move to the next set of columns for the next day
                    column += 2; // Move to the next pair of columns for the next day (Clock-In, Clock-Out)
                }

                int rowIndex = 3;
                foreach (DataRow row in dataTable.Rows)
                {
                    worksheet.Cells[rowIndex, 1].Value = row["EmployeeID"]; 
                    worksheet.Cells[rowIndex, 2].Value = row["EmployeeName"];
                    rowIndex++;
                }

                return package.GetAsByteArray();
            }
        }

        public async Task<ServiceResponse<bool>> ImportBioMetricAttendanceData(
            int instituteID, 
            string employeeID,
            string attendanceDate,
            string clockIn,
            string clockOut)
        {
            try
            {
                await _bioMetricImportRepository.InsertBioMetricAttendanceData(
                    instituteID, 
                    attendanceDate,
                    employeeID,
                    clockIn,
                    clockOut
                );

                return new ServiceResponse<bool>(true, "Attendance data imported successfully", true, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
