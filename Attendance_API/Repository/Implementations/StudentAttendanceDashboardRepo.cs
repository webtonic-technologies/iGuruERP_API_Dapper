using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Repository.Interfaces;
using Dapper;
using System.Data;
using System.Data.Common;

namespace Attendance_API.Repository.Implementations
{
    public class StudentAttendanceDashboardRepo : IStudentAttendanceDashboardRepo
    {
        private readonly IDbConnection _connection;

        public StudentAttendanceDashboardRepo(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<AttendanceCountsDTO>> GetAttendanceCountsForTodayAsync(int instituteId)
        {
            try
            {
                var sql = @"
        DECLARE @Today DATE = CAST(GETDATE() AS DATE);

        -- Count of Present, Absent, and Not Marked
       SELECT
    ISNULL(SUM(CASE WHEN a.Student_Attendance_Status_id = sas.Student_Attendance_Status_id
                     AND sas.Short_Name = 'P' THEN 1 ELSE 0 END), 0) AS PresentCount,
    ISNULL(SUM(CASE WHEN a.Student_Attendance_Status_id = sas.Student_Attendance_Status_id
                     AND sas.Short_Name = 'A' THEN 1 ELSE 0 END), 0) AS AbsentCount,
    ISNULL(SUM(CASE WHEN a.Student_Attendance_Status_id IS NULL THEN 1 ELSE 0 END), 0) AS NotMarkedCount,
    ISNULL(SUM(CASE WHEN a.Student_Attendance_Status_id = sas.Student_Attendance_Status_id
                     AND sas.Short_Name = 'HD' THEN 1 ELSE 0 END), 0) AS HalfDayCount,
    ISNULL(SUM(CASE WHEN a.Student_Attendance_Status_id = sas.Student_Attendance_Status_id
                     AND sas.Short_Name = 'ML' THEN 1 ELSE 0 END), 0) AS MedicalLeaveCount

        FROM tbl_studentmaster s
        LEFT JOIN tbl_StudentAttendanceMaster a
            ON s.Student_id = a.Student_id
            AND a.Date = @Today
        LEFT JOIN tbl_StudentAttendanceStatus sas
            ON a.Student_Attendance_Status_id = sas.Student_Attendance_Status_id
        WHERE a.isDatewise = 1 OR a.Student_id IS NULL
            AND s.Institute_id = @Institute_id;
        ";

                var parameters = new { Institute_id = instituteId };

                var result = await _connection.QuerySingleAsync<AttendanceCountsDTO>(sql, parameters);

                if (result != null)
                {
                    return new ServiceResponse<AttendanceCountsDTO>(true, "Attendance statistics retrieved successfully", result, 200);
                }
                else
                {
                    return new ServiceResponse<AttendanceCountsDTO>(false, "No attendance statistics found for the given parameters", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AttendanceCountsDTO>(false, ex.Message, null, 500);
            }

        }

        public async Task<ServiceResponse<IEnumerable<ClasswiseAttendanceCountsDTO>>> GetClasswiseAttendanceCountsForTodayAsync(int instituteId)
        {
            try
            {
                var sql = @"
        DECLARE @Today DATE = CAST(GETDATE() AS DATE);

        -- Class-wise Count of Present, Absent, and Not Marked
        SELECT
            s.class_id AS ClassId,
            c.class_name AS ClassName,
            ISNULL(SUM(CASE WHEN a.Student_Attendance_Status_id = 1 THEN 1 ELSE 0 END), 0) AS PresentCount,
            ISNULL(SUM(CASE WHEN a.Student_Attendance_Status_id = 2 THEN 1 ELSE 0 END), 0) AS AbsentCount,
            ISNULL(COUNT(CASE WHEN a.Student_Attendance_Status_id IS NULL THEN s.Student_id ELSE NULL END), 0) AS NotMarkedCount
        FROM tbl_studentmaster s
        LEFT JOIN tbl_StudentAttendanceMaster a
            ON s.Student_id = a.Student_id
            AND a.Date = @Today
        LEFT JOIN tbl_Class c
            ON s.class_id = c.class_id
        WHERE (a.isDatewise = 1 OR a.Student_id IS NULL) AND class_name IS NOT NULL
            AND s.Institute_id = @Institute_id
        GROUP BY s.class_id, c.class_name;
        ";

                var parameters = new { Institute_id = instituteId };

                var result = await _connection.QueryAsync<ClasswiseAttendanceCountsDTO>(sql, parameters);

                if (result != null)
                {
                    return new ServiceResponse<IEnumerable<ClasswiseAttendanceCountsDTO>>(true, "Class-wise attendance counts retrieved successfully", result, 200);
                }
                else
                {
                    return new ServiceResponse<IEnumerable<ClasswiseAttendanceCountsDTO>>(false, "No attendance statistics found for the given parameters", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<ClasswiseAttendanceCountsDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<AbsentStudentDTO>>> GetAbsentStudentsForTodayAsync(int instituteId)
        {
            try
            {
                var sql = @"
        DECLARE @Today DATE = CAST(GETDATE() AS DATE);

        -- List of Absent Students
        SELECT
            s.Student_id AS StudentId,
            s.first_name + ' ' + s.last_name AS StudentName,
            c.class_name AS ClassName,
            sec.section_name AS SectionName,
            s.admission_number AS AdmissionNumber
        FROM tbl_studentmaster s
        LEFT JOIN tbl_StudentAttendanceMaster a
            ON s.Student_id = a.Student_id
            AND a.Date = @Today
        LEFT JOIN tbl_StudentAttendanceStatus sas
            ON a.Student_Attendance_Status_id = sas.Student_Attendance_Status_id
        LEFT JOIN tbl_Class c
            ON s.class_id = c.class_id
        LEFT JOIN tbl_Section sec
            ON s.section_id = sec.section_id
        WHERE sas.Short_Name = 'A'
            AND s.Institute_id = @Institute_id;
        ";

                var parameters = new { Institute_id = instituteId };

                var result = await _connection.QueryAsync<AbsentStudentDTO>(sql, parameters);

                if (result != null)
                {
                    return new ServiceResponse<IEnumerable<AbsentStudentDTO>>(true, "Absent students retrieved successfully", result, 200);
                }
                else
                {
                    return new ServiceResponse<IEnumerable<AbsentStudentDTO>>(false, "No absent students found for the given parameters", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<AbsentStudentDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<EmployeeOnLeaveDTO>>> GetEmployeesOnLeaveForTodayAsync(int instituteId)
        {
            try
            {
                var sql = @"
        DECLARE @Today DATE = CAST(GETDATE() AS DATE);

        -- List of Employees on Leave
      SELECT
       e.Employee_id AS EmployeeId,
       e.first_name + ' ' + e.last_name AS EmployeeName,
       d.DepartmentName AS DepartmentName,
       des.DesignationName AS DesignationName,
       a.Remarks AS Remarks
   FROM tbl_EmployeeProfileMaster e
   LEFT JOIN tbl_EmployeeAttendanceMaster a
       ON e.Employee_id = a.Employee_id
       AND a.Date = @Today
   INNER JOIN tbl_EmployeeAttendanceStatusMaster eas
       ON  eas.Employee_Attendance_Status_id = a.Employee_Attendance_Status_id
   LEFT JOIN tbl_Department d
       ON e.department_id = d.Department_id
   LEFT JOIN tbl_Designation des
       ON e.Designation_id = des.Designation_id
   WHERE eas.Short_Name = 'L'
            AND e.Institute_id = @Institute_id;
        ";

                var parameters = new { Institute_id = instituteId };

                var result = await _connection.QueryAsync<EmployeeOnLeaveDTO>(sql, parameters);

                if (result != null)
                {
                    return new ServiceResponse<IEnumerable<EmployeeOnLeaveDTO>>(true, "Employees on leave retrieved successfully", result, 200);
                }
                else
                {
                    return new ServiceResponse<IEnumerable<EmployeeOnLeaveDTO>>(false, "No employees on leave found for the given parameters", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<EmployeeOnLeaveDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<EmployeeAttendanceStatsDTO>> GetEmployeeAttendanceStatsForTodayAsync(int instituteId)
        {
            try
            {
                var sql = @"
        DECLARE @Today DATE = CAST(GETDATE() AS DATE);

        -- Employee Attendance Statistics
        SELECT
            (SELECT COUNT(*) FROM tbl_EmployeeProfileMaster WHERE Institute_id = @Institute_id ) AS TotalEmployeeCount,
            ISNULL(SUM(CASE WHEN a.Employee_Attendance_Status_id = eas.Employee_Attendance_Status_id
                            AND eas.Short_Name = 'P' THEN 1 ELSE 0 END), 0) AS PresentCount,
            ISNULL(SUM(CASE WHEN a.Employee_Attendance_Status_id = eas.Employee_Attendance_Status_id
                            AND eas.Short_Name = 'L' THEN 1 ELSE 0 END), 0) AS OnLeaveCount,
            ISNULL(SUM(CASE WHEN a.Employee_Attendance_Status_id IS NULL THEN 1 ELSE 0 END), 0) AS NotMarkedCount,
            ISNULL(SUM(CASE WHEN a.Date > st.Late_Coming THEN 1 ELSE 0 END), 0) AS LateLoginCount
        FROM tbl_EmployeeProfileMaster e
        LEFT JOIN tbl_EmployeeAttendanceMaster a
            ON e.Employee_id = a.Employee_id
            AND a.Date = @Today
        LEFT JOIN tbl_EmployeeAttendanceStatusMaster eas
            ON a.Employee_Attendance_Status_id = eas.Employee_Attendance_Status_id
        LEFT JOIN tbl_ShiftTimingDesignationMapping stm
            ON e.Designation_id = stm.Designation_id
        LEFT JOIN tbl_ShiftTimingMaster st
            ON stm.Shift_Timing_id = st.Shift_Timing_id
        WHERE e.Institute_id = @Institute_id;
        ";

                var parameters = new { Institute_id = instituteId };

                var result = await _connection.QuerySingleAsync<EmployeeAttendanceStatsDTO>(sql, parameters);

                if (result != null)
                {
                    return new ServiceResponse<EmployeeAttendanceStatsDTO>(true, "Employee attendance statistics retrieved successfully", result, 200);
                }
                else
                {
                    return new ServiceResponse<EmployeeAttendanceStatsDTO>(false, "No employee attendance statistics found for the given parameters", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeAttendanceStatsDTO>(false, ex.Message, null, 500);
            }
        }
    }
}
