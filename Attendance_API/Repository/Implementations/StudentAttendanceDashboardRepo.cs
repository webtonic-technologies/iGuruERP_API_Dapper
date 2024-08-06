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


    }
}
