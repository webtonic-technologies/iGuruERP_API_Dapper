using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.ServiceResponse;
using Attendance_SE_API.Repository.Interfaces;
using OfficeOpenXml;
using System.Threading.Tasks;
using System.Data;
using Attendance_SE_API.Services.Interfaces;

namespace Attendance_SE_API.Services.Implementations
{
    public class EmployeeImportService : IEmployeeImportService
    {
        private readonly IEmployeeImportRepository _employeeImportRepository;

        public EmployeeImportService(IEmployeeImportRepository employeeImportRepository)
        {
            _employeeImportRepository = employeeImportRepository;
        }

        public ServiceResponse<DataTable> DownloadAttendanceTemplate(EmployeeImportRequest request)
        {
            DataTable attendanceData = _employeeImportRepository.GetAttendanceData(request);
            return new ServiceResponse<DataTable>(true, "Attendance Template Retrieved", attendanceData, 200);
        }

        public byte[] GenerateExcelFile(DataTable dataTable, DateTime startDate, DateTime endDate)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Attendance");

                // Adding fixed columns headers
                worksheet.Cells[1, 1].Value = "EmployeeID";
                worksheet.Cells[1, 2].Value = "DepartmentID";
                worksheet.Cells[1, 3].Value = "DesignationID";
                worksheet.Cells[1, 4].Value = "EmployeeCode";
                worksheet.Cells[1, 5].Value = "EmployeeName";
                worksheet.Cells[1, 6].Value = "MobileNumber";

                // Add dynamic columns for each day between startDate and endDate
                int column = 7;  // Start from column G (7th column)
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    worksheet.Cells[1, column].Value = date.ToString("MMM dd, ddd");  // Format: Dec 17, Tue
                    column++;
                }

                // Hide columns A, B, C, and D (EmployeeID, DepartmentID, DesignationID, EmployeeCode)
                worksheet.Column(1).Hidden = true;
                worksheet.Column(2).Hidden = true;
                worksheet.Column(3).Hidden = true;
               

                // Make columns E, F non-editable (EmployeeName and MobileNumber)
                for (int i = 5; i <= 6; i++)  // Columns E (5), F (6)
                {
                    worksheet.Column(i).Style.Locked = true;
                }

                // Populate the data in the Excel sheet
                int rowIndex = 2; // Start from the second row
                foreach (DataRow employeeRow in dataTable.Rows)
                {
                    // Handle DBNull for each value
                    worksheet.Cells[rowIndex, 1].Value = employeeRow != null && employeeRow["EmployeeID"] != DBNull.Value ? employeeRow["EmployeeID"] : "";
                    worksheet.Cells[rowIndex, 2].Value = employeeRow != null && employeeRow["DepartmentID"] != DBNull.Value ? employeeRow["DepartmentID"] : "";
                    worksheet.Cells[rowIndex, 3].Value = employeeRow != null && employeeRow["DesignationID"] != DBNull.Value ? employeeRow["DesignationID"] : "";
                    worksheet.Cells[rowIndex, 4].Value = employeeRow != null && employeeRow["EmployeeCode"] != DBNull.Value ? employeeRow["EmployeeCode"] : "";
                    worksheet.Cells[rowIndex, 5].Value = employeeRow != null && employeeRow["EmployeeName"] != DBNull.Value ? employeeRow["EmployeeName"] : "";
                    worksheet.Cells[rowIndex, 6].Value = employeeRow != null && employeeRow["MobileNumber"] != DBNull.Value ? employeeRow["MobileNumber"] : "";

                    //// Add attendance data for each day from the dynamic columns
                    //column = 7;  // Start from column G (7th column)
                    //for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                    //{
                    //    var statusForDay = employeeRow != null && employeeRow[date.ToString("yyyy-MM-dd")] != DBNull.Value ? employeeRow[date.ToString("yyyy-MM-dd")] : "";
                    //    worksheet.Cells[rowIndex, column].Value = statusForDay;  // Assign attendance status for the day
                    //    column++;
                    //}

                    rowIndex++;
                }

                // Protect the worksheet to prevent changes in locked cells (if needed)
                //worksheet.Protection.IsProtected = true;
                //worksheet.Protection.AllowSelectLockedCells = false;

                // Return the Excel file as a byte array
                return package.GetAsByteArray();
            }
        }


        public DataTable GetStatusData()
        {
            return _employeeImportRepository.GetStatusData();
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
            string designationID,
            string attendanceDate,
            string employeeID,
            string statusID
        )
        {
            try
            {
                // First, delete existing records to avoid duplicates
                await _employeeImportRepository.DeleteEmployeeAttendanceData(
                    instituteID,
                    attendanceDate,
                    departmentID,
                    employeeID
                );

                // Then insert the new data
                await _employeeImportRepository.InsertEmployeeAttendanceData(
                    instituteID,
                    departmentID,
                    designationID,
                    attendanceDate,
                    3, // AttendanceTypeID
                    1, // TimeSlotTypeID
                    false, // IsMarkAsHoliday
                    employeeID,
                    statusID,
                    string.Empty
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
