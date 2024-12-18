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
    public class GeoFencingImportRepository : IGeoFencingImportRepository
    {
        private readonly IDbConnection _dbConnection;

        public GeoFencingImportRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public DataTable GetGeoFencingData(GeoFencingImportRequest request)
        {
            var parameters = new
            {
                InstituteID = request.InstituteID,
                DepartmentID = request.DepartmentID
            };

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
                    WHERE Institute_id = @InstituteID AND Department_id = @DepartmentID
                )
                SELECT 
                    EmployeeID, DepartmentID,
                    DesignationID, EmployeeCode,
                    EmployeeName, MobileNumber
                FROM AttendanceData
                ORDER BY EmployeeName";

            var result = _dbConnection.Query(query, parameters);

            DataTable dataTable = new DataTable();
            if (result.Any())
            {
                var firstRow = result.First();
                foreach (var column in firstRow)
                {
                    dataTable.Columns.Add(column.Key);
                }

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

        public DataTable GetStatusData()
        {
            string query = @"
                SELECT StatusID, StatusName, ShortName 
                FROM tblEmployeeAttendanceStatus 
                WHERE IsActive = 1";

            var result = _dbConnection.Query(query);
            DataTable dataTable = new DataTable();
            if (result.Any())
            {
                var firstRow = result.First();
                foreach (var column in firstRow)
                {
                    dataTable.Columns.Add(column.Key);
                }

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

        //public async Task InsertGeoFencingAttendanceData(
        //    int instituteID,
        //    string departmentID,
        //    string date,
        //    string employeeID,
        //    string clockIn,
        //    string clockOut)
        //{

        //    // Convert the attendance date to the required format (DD-MM-YYYY)
        //    DateTime parsedDate;
        //    if (!DateTime.TryParseExact(date, "MMM dd, ddd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        //    {
        //        throw new ArgumentException("Invalid date format. Expected format is 'MMM dd, ddd' (e.g., 'Dec 17, Tue').");
        //    }

        //    string query = @"
        //        INSERT INTO tblGeoFencingEntry (InstituteID, DepartmentID, EmployeeID, GeoFencingDate, ClockIn, ClockOut)
        //        VALUES (@InstituteID, @DepartmentID, @EmployeeID, @GeoFencingDate, @ClockIn, @ClockOut)";

        //    var parameters = new
        //    {
        //        InstituteID = instituteID,
        //        DepartmentID = departmentID,
        //        EmployeeID = employeeID,
        //        GeoFencingDate = parsedDate.ToString("yyyy-MM-dd"),
        //        ClockIn = clockIn,
        //        ClockOut = clockOut
        //    };

        //    await _dbConnection.ExecuteAsync(query, parameters);
        //}




        public async Task InsertGeoFencingAttendanceData(
    int instituteID,
    string departmentID,
    string date,
    string employeeID,
    string clockIn,
    string clockOut)
        {
            // Convert the attendance date to the required format (DD-MM-YYYY)
            DateTime parsedDate;
            if (!DateTime.TryParseExact(date, "MMM dd, ddd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                throw new ArgumentException("Invalid date format. Expected format is 'MMM dd, ddd' (e.g., 'Dec 17, Tue').");
            }

            // Parse clockIn and clockOut using the 12-hour format with AM/PM (allowing single digit hours)
            TimeSpan parsedClockIn = DateTime.ParseExact(clockIn, "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
            TimeSpan parsedClockOut = DateTime.ParseExact(clockOut, "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;

            // SQL query to insert the data
            string query = @"
    INSERT INTO tblGeoFencingEntry (InstituteID, DepartmentID, EmployeeID, GeoFencingDate, ClockIn, ClockOut)
    VALUES (@InstituteID, @DepartmentID, @EmployeeID, @GeoFencingDate, @ClockIn, @ClockOut)";

            var parameters = new
            {
                InstituteID = instituteID,
                DepartmentID = departmentID,
                EmployeeID = employeeID,
                GeoFencingDate = parsedDate.ToString("yyyy-MM-dd"), // Convert to "yyyy-MM-dd" format
                ClockIn = parsedClockIn,  // TimeSpan to store in SQL Time format
                ClockOut = parsedClockOut  // TimeSpan to store in SQL Time format
            };

            // Execute the query asynchronously
            await _dbConnection.ExecuteAsync(query, parameters);
        }


    }
}
