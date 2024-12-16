using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.Models;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;


namespace Attendance_SE_API.Repository.Implementations
{
    public class EmployeeAttendanceRepository : IEmployeeAttendanceRepository
    {
        private readonly IDbConnection _connection;

        public EmployeeAttendanceRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<bool>> SetAttendance(EmployeeSetAttendanceRequest request)
        {
            try
            {
                foreach (var record in request.AttendanceRecords)
                {
                    // Parse the attendance date to ensure the correct format
                    if (!DateTime.TryParseExact(request.AttendanceDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                    {
                        throw new ArgumentException("Invalid date format. Please use DD-MM-YYYY.");
                    }

                    // Create the attendance object
                    var attendance = new EmployeeAttendance
                    {
                        InstituteID = request.InstituteID,
                        DepartmentID = request.DepartmentID,
                        AttendanceDate = DateTime.ParseExact(request.AttendanceDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString(), // Ensure correct date format
                        TimeSlotTypeID = request.TimeSlotTypeID,
                        IsMarkAsHoliday = request.IsMarkAsHoliday,
                        EmployeeID = record.EmployeeID,
                        StatusID = record.StatusID,
                        Remarks = record.Remarks
                    };

                    // Remove existing records for the same key fields
                    string deleteQuery = @"
                    DELETE FROM tblEmployeeAttendance 
                    WHERE EmployeeID = @EmployeeID
                      AND InstituteID = @InstituteID
                      AND DepartmentID = @DepartmentID
                      AND AttendanceDate = @AttendanceDate
                      AND TimeSlotTypeID = @TimeSlotTypeID";

                    await _connection.ExecuteAsync(deleteQuery, attendance);

                    // Insert the new record
                    string insertQuery = @"
                    INSERT INTO tblEmployeeAttendance (InstituteID, DepartmentID, 
                         AttendanceDate, TimeSlotTypeID, IsMarkAsHoliday, EmployeeID, StatusID, Remarks) 
                    VALUES (@InstituteID, @DepartmentID, @AttendanceDate, 
                            @TimeSlotTypeID, @IsMarkAsHoliday, @EmployeeID, @StatusID, @Remarks)";

                    await _connection.ExecuteAsync(insertQuery, attendance);
                }

                return new ServiceResponse<bool>(true, "Attendance marked successfully.", true, 200);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }


        //public async Task<ServiceResponse<bool>> SetAttendance(EmployeeSetAttendanceRequest request)
        //{
        //    try
        //    {
        //        foreach (var record in request.AttendanceRecords)
        //        {
        //            // Create the attendance object
        //            var attendance = new EmployeeAttendance
        //            {
        //                InstituteID = request.InstituteID,
        //                DepartmentID = request.DepartmentID,
        //                AttendanceDate = DateTime.ParseExact(request.AttendanceDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString(), // Ensure correct date format
        //                TimeSlotTypeID = request.TimeSlotTypeID,
        //                IsMarkAsHoliday = request.IsMarkAsHoliday,
        //                EmployeeID = record.EmployeeID,
        //                StatusID = record.StatusID,
        //                Remarks = record.Remarks
        //            };

        //            // Prepare the SQL insert query
        //            string query = @"INSERT INTO tblEmployeeAttendance (InstituteID, DepartmentID, 
        //                     AttendanceDate, TimeSlotTypeID, IsMarkAsHoliday, EmployeeID, StatusID, Remarks) 
        //                     VALUES (@InstituteID, @DepartmentID, @AttendanceDate, 
        //                     @TimeSlotTypeID, @IsMarkAsHoliday, @EmployeeID, @StatusID, @Remarks)";

        //            // Execute the query
        //            await _connection.ExecuteAsync(query, attendance);
        //        }

        //        return new ServiceResponse<bool>(true, "Attendance marked successfully.", true, 200);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception (optional)
        //        return new ServiceResponse<bool>(false, ex.Message, false, 500);
        //    }
        //}


        public async Task<ServiceResponse<List<EmployeeAttendanceResponse>>> GetAttendance_EMP(GetEmployeeAttendanceRequest request)
        {
            // Parse the attendance date from DD-MM-YYYY format
            DateTime parsedDate;
            if (!DateTime.TryParseExact(request.AttendanceDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return new ServiceResponse<List<EmployeeAttendanceResponse>>(false, "Invalid date format. Please use DD-MM-YYYY.", null, 400);
            }

            // Query to fetch attendance data
            string query = @"
        DECLARE @AttendanceDate DATE = @AttendanceDateParam;  
        DECLARE @DepartmentID INT = @DepartmentIDParam;                     
        DECLARE @InstituteID INT = @InstituteIDParam;  
        DECLARE @TimeSlotTypeID INT = @TimeSlotTypeIDParam;        

        ------------Part I------------
        WITH PartI AS (
            SELECT 
                e.Employee_id, 
                e.Employee_code_id,  
                CONCAT(e.First_Name, ' ', e.Last_Name) AS EmployeeName, 
                d.DepartmentName AS Department, -- Include Department Name
                ISNULL(a.StatusID, 0) AS StatusID, 
                ISNULL(a.Remarks, '') AS Remarks 
            FROM tbl_EmployeeProfileMaster e
            LEFT JOIN tblEmployeeAttendance a ON e.Employee_id = a.EmployeeID
                AND a.AttendanceDate = @AttendanceDate
            LEFT JOIN tbl_Department d ON e.Department_id = d.Department_id -- Join with Department table
            WHERE e.Department_id = @DepartmentID  
                AND e.Institute_id = @InstituteID
                AND (@TimeSlotTypeID IS NULL OR a.TimeSlotTypeID = @TimeSlotTypeID)
        )

        ------------Part II------------
        SELECT 
            e.Employee_id, 
            e.Employee_code_id,   
            CONCAT(e.First_Name, ' ', e.Last_Name) AS EmployeeName, 
            d.DepartmentName AS Department, -- Include Department Name
            '' AS StatusID, 
            '' AS Remarks 
        FROM tbl_EmployeeProfileMaster e
        LEFT JOIN tbl_Department d ON e.Department_id = d.Department_id -- Join with Department table
        WHERE e.Department_id = @DepartmentID   
            AND e.Institute_id = @InstituteID
            AND e.Employee_id NOT IN (SELECT Employee_id FROM PartI)  -- Exclude employees in Part I

        ------------Combine both parts using UNION------------
        UNION

        SELECT 
            e.Employee_id, 
            e.Employee_code_id,   
            CONCAT(e.First_Name, ' ', e.Last_Name) AS EmployeeName, 
            d.DepartmentName AS Department, -- Include Department Name
            ISNULL(a.StatusID, 0) AS StatusID, 
            ISNULL(a.Remarks, '') AS Remarks 
        FROM tbl_EmployeeProfileMaster e
        LEFT JOIN tblEmployeeAttendance a ON e.Employee_id = a.EmployeeID
            AND a.AttendanceDate = @AttendanceDate
        LEFT JOIN tbl_Department d ON e.Department_id = d.Department_id -- Join with Department table
        WHERE e.Department_id = @DepartmentID   
            AND e.Institute_id = @InstituteID
            AND (@TimeSlotTypeID IS NULL OR a.TimeSlotTypeID = @TimeSlotTypeID);";

            // Set parameters for the SQL query
            var parameters = new
            {
                AttendanceDateParam = parsedDate,
                DepartmentIDParam = request.DepartmentID,
                InstituteIDParam = request.InstituteID,
                TimeSlotTypeIDParam = request.TimeSlotTypeID // This parameter is also included
            };

            // Execute the query
            var attendanceRecords = await _connection.QueryAsync<EmployeeAttendanceResponse>(query, parameters);

            // Create the final response
            var response = new ServiceResponse<List<EmployeeAttendanceResponse>>(
                success: true,
                message: "Attendance fetched successfully.",
                data: attendanceRecords.AsList(),
                statusCode: 200,
                totalCount: attendanceRecords.Count()
            );

            return response;
        }


        //public async Task<ServiceResponse<List<EmployeeAttendanceResponse>>> GetAttendance_EMP(GetEmployeeAttendanceRequest request)
        //{
        //    // Convert the attendance date from DD-MM-YYYY format to a SQL-compatible format
        //    DateTime parsedDate;
        //    if (!DateTime.TryParseExact(request.AttendanceDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        //    {
        //        return new ServiceResponse<List<EmployeeAttendanceResponse>>(false, "Invalid date format. Please use DD-MM-YYYY.", null, 400);
        //    }

        //    // Query to fetch attendance data
        //    string query = @"
        //    WITH AttendanceData AS (
        //        SELECT 
        //            e.Employee_id, 
        //            e.Employee_code_id,  
        //            CONCAT(e.First_Name, ' ', e.Last_Name) AS EmployeeName, 
        //            a.AttendanceDate,
        //            CASE 
        //                WHEN a.StatusID = 1 THEN 'P'
        //                WHEN a.StatusID = 2 THEN 'A'
        //                WHEN a.StatusID = 3 THEN 'HD'
        //                WHEN a.StatusID = 4 THEN 'L'
        //                WHEN a.StatusID = 5 THEN 'SL'
        //                WHEN a.StatusID = 6 THEN 'TL'
        //                ELSE '-'
        //            END AS AttendanceStatus
        //        FROM tbl_EmployeeProfileMaster e
        //        LEFT JOIN tblEmployeeAttendance a ON e.Employee_id = a.EmployeeID
        //            AND a.AttendanceDate = @AttendanceDate
        //        WHERE e.Department_id = @DepartmentID  
        //            AND e.Institute_id = @InstituteID
        //    )
        //    SELECT 
        //        Employee_id,
        //        Employee_code_id, 
        //        EmployeeName,
        //        AttendanceDate,
        //        AttendanceStatus
        //    FROM AttendanceData
        //    ORDER BY EmployeeName, AttendanceDate;";

        //    // Set parameters for the SQL query
        //    var parameters = new
        //    {
        //        AttendanceDate = parsedDate,
        //        DepartmentID = request.DepartmentID,
        //        InstituteID = request.InstituteID
        //    };

        //    // Execute the query
        //    var attendanceRecords = await _connection.QueryAsync<EmployeeAttendanceResponse>(query, parameters);

        //    return new ServiceResponse<List<EmployeeAttendanceResponse>>(true, "Attendance fetched successfully.", attendanceRecords.AsList(), 200);
        //}

        public async Task<List<Department>> GetDepartmentsByInstituteAsync(int instituteId)
        {
            // Use the injected _connection object directly instead of _connectionString
                var query = @"
            SELECT Department_id AS DepartmentId, DepartmentName
            FROM tbl_Department
            WHERE Institute_id = @InstituteID AND IsDeleted = 0";  // Remove IsDeleted from SELECT

            // Execute the query using _connection
            var departments = await _connection.QueryAsync<Department>(query, new { InstituteID = instituteId });

            return departments.ToList();
        }


    }
}
