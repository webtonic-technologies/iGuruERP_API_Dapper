using Dapper;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.Repository.Interfaces;
using System.Globalization;

namespace Attendance_SE_API.Repository.Implementations
{
    public class EmployeeImportRepository : IEmployeeImportRepository
    {
        private readonly IDbConnection _dbConnection;

        public EmployeeImportRepository(IConfiguration configuration)
        {
            // Initialize the database connection using the connection string
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        // Method to fetch attendance data
        public DataTable GetAttendanceData(EmployeeImportRequest request)
        {
            // Ensure valid dates are parsed
            if (string.IsNullOrEmpty(request.StartDate) || string.IsNullOrEmpty(request.EndDate))
            {
                throw new ArgumentException("StartDate and EndDate must not be null or empty.");
            }

            var parameters = new
            {
                DepartmentID = request.DepartmentID,
                InstituteID = request.InstituteID,
                StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)
            };

            // SQL query to fetch the attendance data
            string query = @"
                WITH AttendanceData AS (
                    SELECT 
                        Employee_id AS EmployeeID, 
                        Department_id AS DepartmentID,
                        Designation_id AS DesignationID, 
                        Employee_code_id AS EmployeeCode, 
                        CONCAT(First_Name, ' ', Last_Name) AS EmployeeName, 
                        Mobile_Number AS MobileNumber
                    FROM tbl_EmployeeProfileMaster
                    WHERE Department_id = @DepartmentID 
                        AND Institute_id = @InstituteID
                )
                SELECT 
                    EmployeeID, DepartmentID,
                    DesignationID, EmployeeCode,
                    EmployeeName, MobileNumber
                FROM AttendanceData
                ORDER BY EmployeeName";

            // Execute the query and return the result as a DataTable
            var result = _dbConnection.Query(query, parameters);

            // Create a new DataTable to hold the results
            DataTable dataTable = new DataTable();
            if (result.Any())
            {
                var firstRow = result.First();
                foreach (var column in firstRow)
                {
                    dataTable.Columns.Add(column.Key);
                }

                // Add rows to the DataTable
                foreach (var row in result)
                {
                    var dataRow = dataTable.NewRow();
                    foreach (var column in row)
                    {
                        dataRow[column.Key] = column.Value;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

        // Method to fetch status data for Sheet 2 (e.g., Attendance Status)
        public DataTable GetStatusData()
        {
            // SQL query to fetch status data (active, default, or specific to InstituteID)
            string query = @"
                SELECT StatusID, StatusName, ShortName 
                FROM tblEmployeeAttendanceStatus 
                WHERE IsActive = 1 AND IsDefault = 1
                UNION ALL 
                SELECT StatusID, StatusName, ShortName 
                FROM tblEmployeeAttendanceStatus 
                WHERE IsActive = 1 AND IsDefault = 0 AND InstituteID = 6";

            // Execute the query and return the result as a DataTable
            var result = _dbConnection.Query(query);

            // Create a new DataTable to hold the results
            DataTable dataTable = new DataTable();
            if (result.Any())
            {
                var firstRow = result.First();
                foreach (var column in firstRow)
                {
                    dataTable.Columns.Add(column.Key);
                }

                // Add rows to the DataTable
                foreach (var row in result)
                {
                    var dataRow = dataTable.NewRow();
                    foreach (var column in row)
                    {
                        dataRow[column.Key] = column.Value;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

        // Method to delete existing employee attendance data
        public async Task DeleteEmployeeAttendanceData(
            int instituteID,
            string attendanceDate,
            string departmentID,
            string employeeID)
        {
            // Convert the attendance date to the required format (DD-MM-YYYY)
            DateTime parsedDate;
            if (!DateTime.TryParseExact(attendanceDate, "MMM dd, ddd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                throw new ArgumentException("Invalid date format. Expected format is 'MMM dd, ddd' (e.g., 'Dec 17, Tue').");
            }

            string query = @"
                DELETE FROM tblEmployeeAttendance 
                WHERE EmployeeID = @EmployeeID
                    AND AttendanceDate = @AttendanceDate 
                    AND DepartmentID = @DepartmentID 
                    AND InstituteID = @InstituteID";

            await _dbConnection.ExecuteAsync(query, new
            {
                InstituteID = instituteID,
                AttendanceDate = parsedDate.ToString("yyyy-MM-dd"),
                DepartmentID = departmentID,
                EmployeeID = employeeID
            });
        }

        // Method to insert new employee attendance data
        public async Task InsertEmployeeAttendanceData(
            int instituteID,
            string departmentID,
            string designationID,
            string attendanceDate,
            int attendanceTypeID,
            int timeSlotTypeID,
            bool isMarkAsHoliday,
            string employeeID,
            string statusID,
            string remarks)
        {
            // Convert the attendance date to the required format (DD-MM-YYYY)
            DateTime parsedDate;
            if (!DateTime.TryParseExact(attendanceDate, "MMM dd, ddd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                throw new ArgumentException("Invalid date format. Expected format is 'MMM dd, ddd' (e.g., 'Dec 17, Tue').");
            }

            string query = @"
                INSERT INTO tblEmployeeAttendance 
                (InstituteID, DepartmentID, 
                 AttendanceDate, TimeSlotTypeID, IsMarkAsHoliday, 
                 EmployeeID, StatusID, Remarks, IsImported)
                VALUES 
                (@InstituteID, @DepartmentID, 
                 @AttendanceDate, @TimeSlotTypeID, @IsMarkAsHoliday, 
                 @EmployeeID, @StatusID, @Remarks, 1)";

            await _dbConnection.ExecuteAsync(query, new
            {
                InstituteID = instituteID,
                AttendanceTypeID = attendanceTypeID,
                DepartmentID = departmentID,
                AttendanceDate = parsedDate.ToString("yyyy-MM-dd"),
                TimeSlotTypeID = timeSlotTypeID,
                IsMarkAsHoliday = isMarkAsHoliday,
                EmployeeID = employeeID,
                StatusID = statusID,
                Remarks = remarks
            });
        }
    }
}
