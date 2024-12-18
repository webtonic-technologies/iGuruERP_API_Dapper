using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.ServiceResponse;
using Attendance_SE_API.Repository.Interfaces;
using OfficeOpenXml;
using System.Threading.Tasks;
using System.Data;
using Attendance_SE_API.Services.Interfaces;

namespace Attendance_SE_API.Services.Implementations
{
    public class GeoFencingImportService : IGeoFencingImportService
    {
        private readonly IGeoFencingImportRepository _geoFencingImportRepository;

        public GeoFencingImportService(IGeoFencingImportRepository geoFencingImportRepository)
        {
            _geoFencingImportRepository = geoFencingImportRepository;
        }

        public ServiceResponse<DataTable> DownloadGeoFencingTemplate(GeoFencingImportRequest request)
        {
            DataTable geoFencingData = _geoFencingImportRepository.GetGeoFencingData(request);
            return new ServiceResponse<DataTable>(true, "GeoFencing Template Retrieved", geoFencingData, 200);
        }

        public byte[] GenerateExcelFile(DataTable dataTable, DateTime startDate, DateTime endDate)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("GeoFencing");

                // Adding fixed columns headers
                worksheet.Cells[1, 1].Value = "EmployeeID";
                worksheet.Cells[1, 2].Value = "DepartmentID";
                worksheet.Cells[1, 3].Value = "DesignationID";
                worksheet.Cells[1, 4].Value = "EmployeeCode";
                worksheet.Cells[1, 5].Value = "EmployeeName";
                worksheet.Cells[1, 6].Value = "MobileNumber";

                // Adding day columns with Clock-In and Clock-Out under each date
                int column = 7;
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

                // Populate the data in the Excel sheet
                int rowIndex = 3; // Starting from row 3 as row 1 and 2 are headers
                foreach (DataRow employeeRow in dataTable.Rows)
                {
                    worksheet.Cells[rowIndex, 1].Value = employeeRow["EmployeeID"];
                    worksheet.Cells[rowIndex, 2].Value = employeeRow["DepartmentID"];
                    worksheet.Cells[rowIndex, 3].Value = employeeRow["DesignationID"];
                    worksheet.Cells[rowIndex, 4].Value = employeeRow["EmployeeCode"];
                    worksheet.Cells[rowIndex, 5].Value = employeeRow["EmployeeName"];
                    worksheet.Cells[rowIndex, 6].Value = employeeRow["MobileNumber"];

                    // Start populating the clock-in and clock-out values for each day
                    int dailyColumnIndex = 7;
                    for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                    {
                        // Placeholder for Clock-In and Clock-Out values
                        worksheet.Cells[rowIndex, dailyColumnIndex].Value = ""; // Clock-In placeholder
                        worksheet.Cells[rowIndex, dailyColumnIndex + 1].Value = ""; // Clock-Out placeholder

                        dailyColumnIndex += 2; // Move to the next set of columns for the next day
                    }

                    rowIndex++;
                }

                return package.GetAsByteArray();
            }
        }
         
        public DataTable GetStatusData()
        {
            return _geoFencingImportRepository.GetStatusData();
        }

        public byte[] AddStatusSheetToExcel(byte[] excelFile, DataTable statusData)
        {
            using (var package = new ExcelPackage(new System.IO.MemoryStream(excelFile)))
            {
                var worksheet = package.Workbook.Worksheets.Add("Status");
                worksheet.Cells.LoadFromDataTable(statusData, true);
                return package.GetAsByteArray();
            }
        }


        public async Task<ServiceResponse<bool>> ImportAttendanceData(
            int instituteID,
            string departmentID, 
            string date,
            string employeeID,
            string clockIn,
            string clockOut)
        {
            try
            {
                // Call the repository method to insert attendance data
                await _geoFencingImportRepository.InsertGeoFencingAttendanceData(
                    instituteID,
                    departmentID, 
                    date,
                    employeeID,
                    clockIn,
                    clockOut
                );

                return new ServiceResponse<bool>(true, "Attendance data imported successfully", true, 200);
            }
            catch (System.Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
