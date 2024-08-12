using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Repository.Interfaces;
using Dapper;

namespace Attendance_API.Repository.Implementations
{
    public class StudentAttendanceMasterRepository : IStudentAttendanceMasterRepository
    {
        private readonly IDbConnection _connection;

        public StudentAttendanceMasterRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<StudentAttendanceMasterResponseDTO>> GetStudentAttendanceMasterList(StudentAttendanceMasterRequestDTO request)
        {
            if (request == null || request.Date == DateTime.MinValue || request.class_id == 0 || request.section_id == 0)
            {
                return new ServiceResponse<StudentAttendanceMasterResponseDTO>(false, "Invalid request", new StudentAttendanceMasterResponseDTO(), 400);
            }
            string sql = $"SELECT sam.Student_Attendance_id, sam.Student_Attendance_Status_id, sam.Remark, '{request.Date.ToString("yyyy-MM-dd")}' as Date, sm.student_id as Student_id, sm.First_Name + ' ' + sm.Last_Name AS Student_Name, " +
                         $"sm.Admission_Number, sm.Roll_Number, sas.Student_Attendance_Status_Type, sas.Short_Name as Student_Attendance_Status_Short_Name " +
                         $"FROM tbl_StudentMaster sm " +
                         $"LEFT JOIN tbl_StudentAttendanceMaster sam ON sam.Student_id = sm.student_id and sam.Date = '{request.Date.ToString("yyyy-MM-dd")}' " +
                         $"join tbl_StudentAttendanceStatus sas on sas.Student_Attendance_Status_id = sam.Student_Attendance_Status_id " +
                         $"where sm.class_id = {request.class_id} and sm.section_id = {request.section_id} AND sm.Institute_id = {request.Institute_id} AND isDatewise = {request.isDatewise}";

            if (request.pageNumber != null && request.pageSize != null)
            {
                sql += $" Order by 1 OFFSET {(request.pageNumber - 1) * request.pageSize} ROWS FETCH NEXT {request.pageSize} ROWS ONLY;";
            }

            var result = await _connection.QueryAsync<StudentAttendanceMasterResponse>(sql);

            sql = $"SELECT COUNT(*) " +
                         $"FROM tbl_StudentMaster sm " +
                         $"LEFT JOIN tbl_StudentAttendanceMaster sam ON sam.Student_id = sm.student_id and sam.Date = '{request.Date.ToString("yyyy-MM-dd")}' " +
                         $"join tbl_StudentAttendanceStatus sas on sas.Student_Attendance_Status_id = sam.Student_Attendance_Status_id " +
                         $"where sm.class_id = {request.class_id} and sm.section_id = {request.section_id} AND sm.Institute_id = {request.Institute_id} AND isDatewise = {request.isDatewise}";

            var countRes = await _connection.QueryAsync<long>(sql);
            var count = countRes.FirstOrDefault();

            return new ServiceResponse<StudentAttendanceMasterResponseDTO>(true, "Operation successful", new StudentAttendanceMasterResponseDTO{ Data = result.ToList(), Total = count}, 200);
        }

        public async Task<ServiceResponse<StudentAttendanceMasterDTO>> InsertOrUpdateStudentAttendanceMaster(StudentAttendanceMasterDTO studentAttendanceMaster)
        {
            if (studentAttendanceMaster == null || studentAttendanceMaster.Student_id == 0 || studentAttendanceMaster.Date == DateTime.MinValue || studentAttendanceMaster.Student_Attendance_Status_id == 0)
            {
                return new ServiceResponse<StudentAttendanceMasterDTO>(false, "Invalid request", null, 400);
            }

            if (studentAttendanceMaster.Student_Attendance_id == 0)
            {
                // Insert new record
                string insertSql = $"INSERT INTO tbl_StudentAttendanceMaster (Student_id, Student_Attendance_Status_id, Remark, Date,isHoliday,TimeSlot_id,Subject_id,isDatewise) " +
                                  $"VALUES (@Student_id, @Student_Attendance_Status_id, @Remark, @Date,@isHoliday,@TimeSlot_id,@Subject_id,@isDatewise)";
                await _connection.ExecuteAsync(insertSql, studentAttendanceMaster);
            }
            else
            {
                // Update existing record
                string updateSql = $"UPDATE tbl_StudentAttendanceMaster " +
                                  $"SET Student_id = @Student_id, Student_Attendance_Status_id = @Student_Attendance_Status_id, Remark = @Remark, Date = @Date " +
                                  $"WHERE Student_Attendance_id = @Student_Attendance_id , isHoliday = @isHoliday,TimeSlot_id = @TimeSlot_id , Subject_id = @Subject_id , isDatewise = @isDatewise";
                await _connection.ExecuteAsync(updateSql, studentAttendanceMaster);
            }

            return new ServiceResponse<StudentAttendanceMasterDTO>(true, "Operation successful", studentAttendanceMaster, 200);
        }

        public async Task<ServiceResponse<bool>> DeleteStudentAttendanceMaster(int studentAttendanceId)
        {
            if (studentAttendanceId == 0)
            {
                return new ServiceResponse<bool>(false, "Invalid request", false, 400);
            }

            string deleteSql = $"DELETE FROM tbl_StudentAttendanceMaster WHERE Student_Attendance_id = @Student_Attendance_id";
            await _connection.ExecuteAsync(deleteSql, new { Student_Attendance_id = studentAttendanceId });

            return new ServiceResponse<bool>(true, "Operation successful", true, 200);
        }
    }
}
