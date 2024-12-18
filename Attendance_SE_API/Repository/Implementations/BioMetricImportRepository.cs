using System.Data;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Attendance_SE_API.DTOs.Requests;
using System.Globalization;
using Attendance_SE_API.Repository.Interfaces;

namespace Attendance_SE_API.Repository.Implementations
{
    public class BioMetricImportRepository : IBioMetricImportRepository
    {
        private readonly IDbConnection _dbConnection;

        public BioMetricImportRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public DataTable GetBioMetricData(BioMetricImportRequest request)
        {
            var parameters = new { InstituteID = request.InstituteID };

            string query = @"
                    SELECT 
                    Employee_id AS EmployeeID,  
                    Employee_code_id AS EmployeeCode, 
                    CONCAT(First_Name, ' ', Last_Name) AS EmployeeName
                FROM tbl_EmployeeProfileMaster
                WHERE Institute_id = @InstituteID";


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

            //var result = _dbConnection.Query(query, parameters);
            //DataTable dataTable = new DataTable();

            //foreach (var column in result.FirstOrDefault()?.Keys)
            //{
            //    dataTable.Columns.Add(column);
            //}

            //foreach (var row in result)
            //{
            //    DataRow dataRow = dataTable.NewRow();
            //    foreach (var column in row)
            //    {
            //        dataRow[column.Key] = column.Value;
            //    }
            //    dataTable.Rows.Add(dataRow);
            //}

            //return dataTable;
        }

        public async Task InsertBioMetricAttendanceData(
            int instituteID, 
            string date,
            string employeeID,
            string clockIn,
            string clockOut)
        {
            DateTime parsedDate = DateTime.ParseExact(date, "MMM dd, ddd", CultureInfo.InvariantCulture);
            string query = @"
                INSERT INTO tblBioMericAttendance (EmployeeID, AttendanceDate, ClockIn, ClockOut, InstituteID)
                VALUES (@EmployeeID, @AttendanceDate, @ClockIn, @ClockOut, @InstituteID)";

            var parameters = new
            {
                InstituteID = instituteID, 
                EmployeeID = employeeID,
                AttendanceDate = parsedDate.ToString("yyyy-MM-dd"),
                ClockIn = clockIn,
                ClockOut = clockOut
            };

            await _dbConnection.ExecuteAsync(query, parameters);
        }
    }
}
