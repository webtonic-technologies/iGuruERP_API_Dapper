﻿using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Attendance_SE_API.DTOs.Response;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Attendance_SE_API.Repository.Implementations
{
    public class AttendanceDashboardRepository : IAttendanceDashboardRepository
    {
        private readonly IConfiguration _config;

        public AttendanceDashboardRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<ServiceResponse<DashboardAttendanceStatisticsResponse>> GetStudentAttendanceStatistics(int instituteId, string AcademicYearCode)
        {
            string query = @"
            SELECT 
                (SELECT COUNT(DISTINCT sa.StudentID) 
                 FROM tblStudentAttendance sa 
                 WHERE sa.StatusID = 1 
                 AND sa.AttendanceTypeID = 1
                 AND sa.AttendanceDate = CAST(GETDATE() AS DATE)
                 AND sa.InstituteID = @InstituteID AND sa.AcademicYearCode = @AcademicYearCode) AS NoOfPresent,

                (SELECT COUNT(DISTINCT sa.StudentID) 
                 FROM tblStudentAttendance sa 
                 WHERE sa.StatusID = 2 
                 AND sa.AttendanceTypeID = 1
                 AND sa.AttendanceDate = CAST(GETDATE() AS DATE)
                 AND sa.InstituteID = @InstituteID AND sa.AcademicYearCode = @AcademicYearCode) AS NoOfAbsent,

                (SELECT COUNT(DISTINCT sm.student_id) 
                 FROM tbl_StudentMaster sm
                 WHERE NOT EXISTS (
                     SELECT 1
                     FROM tblStudentAttendance sa
                     WHERE sa.StudentID = sm.student_id 
                     AND sa.AttendanceTypeID = 1
                     AND sa.AttendanceDate = CAST(GETDATE() AS DATE)
                     AND sa.InstituteID = @InstituteID AND sa.AcademicYearCode = @AcademicYearCode
                 )) AS NoOfNotMarked";

            try
            {
                // Use _config to get the connection string
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    var result = await connection.QueryFirstOrDefaultAsync(query, new { InstituteID = instituteId, AcademicYearCode = AcademicYearCode });

                    var response = new DashboardAttendanceStatisticsResponse
                    {
                        NoOfPresent = result?.NoOfPresent ?? 0,
                        NoOfAbsent = result?.NoOfAbsent ?? 0,
                        NoOfNotMarked = result?.NoOfNotMarked ?? 0
                    };

                    return new ServiceResponse<DashboardAttendanceStatisticsResponse>(
                        true,
                        "Successfully fetched student attendance statistics.",
                        response,
                        200
                    );
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<DashboardAttendanceStatisticsResponse>(
                    false,
                    "An error occurred while fetching attendance statistics: " + ex.Message,
                    null,
                    500
                );
            }
        }


        public async Task<ServiceResponse<List<GetStudentAttendanceDashboardResponse>>> GetStudentAttendanceDashboard(int instituteId, string AcademicYearCode)
        {
            string query = @"
    SELECT 
        c.class_id AS ClassID,
        c.class_name AS ClassName,

        (SELECT COUNT(DISTINCT sa.StudentID) 
         FROM tblStudentAttendance sa 
         WHERE sa.StatusID = 1 
         AND sa.AttendanceTypeID = 1
         AND sa.AttendanceDate = CAST(GETDATE() AS DATE)
         AND sa.ClassID = c.class_id
         AND sa.InstituteID = @InstituteID AND sa.AcademicYearCode = @AcademicYearCode) AS Present,

        (SELECT COUNT(DISTINCT sa.StudentID) 
         FROM tblStudentAttendance sa 
         WHERE sa.StatusID = 2 
         AND sa.AttendanceTypeID = 1
         AND sa.AttendanceDate = CAST(GETDATE() AS DATE)
         AND sa.ClassID = c.class_id
         AND sa.InstituteID = @InstituteID AND sa.AcademicYearCode = @AcademicYearCode) AS Absent,

        (SELECT COUNT(DISTINCT sm.student_id) 
         FROM tbl_StudentMaster sm
         WHERE sm.class_id = c.class_id
         AND NOT EXISTS (
             SELECT 1
             FROM tblStudentAttendance sa
             WHERE sa.StudentID = sm.student_id
             AND sa.AttendanceDate = CAST(GETDATE() AS DATE)
             AND sa.InstituteID = @InstituteID AND sa.AcademicYearCode = @AcademicYearCode
         )) AS NotMarked
    FROM 
        tbl_Class c
    WHERE 
        c.institute_id = @InstituteID
    ORDER BY 
        c.class_id";

            try
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    // QueryAsync returns an IEnumerable<T> collection, so we convert it to a List
                    var result = await connection.QueryAsync<GetStudentAttendanceDashboardResponse>(query, new { InstituteID = instituteId, AcademicYearCode = AcademicYearCode });

                    // Return the result as a list in the ServiceResponse
                    return new ServiceResponse<List<GetStudentAttendanceDashboardResponse>>(
                        true,
                        "Successfully fetched student attendance dashboard data.",
                        result.ToList(),
                        200
                    );
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetStudentAttendanceDashboardResponse>>(
                    false,
                    "An error occurred while fetching attendance dashboard data: " + ex.Message,
                    null,
                    500
                );
            }
        }


        public async Task<ServiceResponse<GetEmployeeAttendanceStatisticsResponse>> GetEmployeeAttendanceStatistics(int instituteId)
        {
            string query = @"
                SELECT 
                    (SELECT COUNT(*) 
                     FROM tbl_EmployeeProfileMaster e 
                     WHERE e.Status = 1 AND e.Institute_id = @InstituteID) AS TotalEmployeeCount,
                     
                    (SELECT COUNT(DISTINCT ea.EmployeeID)
                     FROM tblEmployeeAttendance ea
                     WHERE ea.StatusID = 1 
                     AND ea.AttendanceDate = CAST(GETDATE() AS DATE)
                     AND ea.InstituteID = @InstituteID) AS Present,
                     
                    (SELECT COUNT(DISTINCT ea.EmployeeID)
                     FROM tblEmployeeAttendance ea
                     WHERE ea.StatusID IN (4, 5) 
                     AND ea.AttendanceDate = CAST(GETDATE() AS DATE)
                     AND ea.InstituteID = @InstituteID) AS OnLeave,
                     
                    (SELECT COUNT(DISTINCT e.Employee_id)
                     FROM tbl_EmployeeProfileMaster e
                     LEFT JOIN tblEmployeeAttendance ea ON e.Employee_id = ea.EmployeeID
                     WHERE ea.EmployeeID IS NULL 
                     AND e.Status = 1
                     AND e.Institute_id = @InstituteID) AS NotInYet";

            try
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    var result = await connection.QueryFirstOrDefaultAsync<GetEmployeeAttendanceStatisticsResponse>(query, new { InstituteID = instituteId });

                    return new ServiceResponse<GetEmployeeAttendanceStatisticsResponse>(
                        true,
                        "Successfully fetched employee attendance statistics.",
                        result,
                        200
                    );
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetEmployeeAttendanceStatisticsResponse>(
                    false,
                    "An error occurred while fetching employee attendance statistics: " + ex.Message,
                    null,
                    500
                );
            }
        }

        public async Task<ServiceResponse<List<GetEmployeeOnLeaveResponse>>> GetEmployeeOnLeave(int instituteId)
        {
            string query = @"
                SELECT 
                    e.First_Name + ' ' + e.Middle_Name + ' ' + e.Last_Name AS EmployeeName,
                    d.DepartmentName AS Department,
                    des.DesignationName AS Designation,
                    ea.Remarks AS Reason
                FROM 
                    tblEmployeeAttendance ea
                INNER JOIN 
                    tbl_EmployeeProfileMaster e ON ea.EmployeeID = e.Employee_id
                INNER JOIN 
                    tbl_Department d ON e.Department_id = d.Department_id
                INNER JOIN 
                    tbl_Designation des ON e.Designation_id = des.Designation_id
                INNER JOIN 
                    tblEmployeeAttendanceStatus es ON ea.StatusID = es.StatusID
                WHERE 
                    ea.StatusID IN (2, 4)  -- Absent or Medical Leave
                    AND ea.AttendanceDate = CAST(GETDATE() AS DATE)
                    AND ea.InstituteID = @InstituteID
                ORDER BY 
                    e.First_Name, e.Last_Name";

            try
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    var result = await connection.QueryAsync<GetEmployeeOnLeaveResponse>(query, new { InstituteID = instituteId });

                    return new ServiceResponse<List<GetEmployeeOnLeaveResponse>>(
                        true,
                        "Successfully fetched employees on leave.",
                        result.AsList(),
                        200
                    );
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetEmployeeOnLeaveResponse>>(
                    false,
                    "An error occurred while fetching employees on leave: " + ex.Message,
                    null,
                    500
                );
            }
        }

    }
}
