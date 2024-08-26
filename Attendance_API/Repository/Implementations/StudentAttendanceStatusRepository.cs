using Dapper;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Models;
using Attendance_API.Repository.Interfaces;
using System.Data;

namespace Attendance_API.Repository.Implementations
{
    public class StudentAttendanceStatusRepository : IStudentAttendanceStatusRepository
    {
        private readonly IDbConnection _connection;

        public StudentAttendanceStatusRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<StudentAttendanceStatusDTO>>> GetStudentAttendanceStatusList(int InstituteId)
        {
            try
            {
                var response = new List<StudentAttendanceStatusDTO>();
                string sql = @"SELECT *
                       FROM [dbo].[tbl_StudentAttendanceStatus] where isDelete = 0 AND InstituteId = @InstituteId";

                // Execute the query and retrieve the student attendance status
                var data = await _connection.QueryAsync<StudentAttendanceStatus>(sql, new { InstituteId });
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var record = new StudentAttendanceStatusDTO
                        {
                            Student_Attendance_Status_id = item.Student_Attendance_Status_id,
                            Student_Attendance_Status_Type = item.Student_Attendance_Status_Type,
                            Short_Name = item.Short_Name
                        };
                        response.Add(record);
                    }
                    return new ServiceResponse<List<StudentAttendanceStatusDTO>>(true, "Record found", response, 200);
                }
                else
                {
                    return new ServiceResponse<List<StudentAttendanceStatusDTO>>(false, "record not found", new List<StudentAttendanceStatusDTO>(), 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentAttendanceStatusDTO>>(false, ex.Message, new List<StudentAttendanceStatusDTO>(), 500);
            }
        }

        public async Task<ServiceResponse<StudentAttendanceStatusDTO>> GetStudentAttendanceStatusById(int Student_Attendance_Status_id)
        {
            try
            {
                var response = new StudentAttendanceStatusDTO();
                string sql = @"SELECT *
                       FROM [dbo].[tbl_StudentAttendanceStatus]
                       WHERE Student_Attendance_Status_id = @Student_Attendance_Status_id and isDelete = 0";

                // Execute the query and retrieve the student attendance status
                var data = await _connection.QueryFirstOrDefaultAsync<StudentAttendanceStatus>(sql, new { Student_Attendance_Status_id });
                if (data != null)
                {
                    response.Student_Attendance_Status_id = data.Student_Attendance_Status_id;
                    response.Student_Attendance_Status_Type = data.Student_Attendance_Status_Type;
                    response.Short_Name = data.Short_Name;
                    return new ServiceResponse<StudentAttendanceStatusDTO>(true, "Record found", response, 200);
                }
                else
                {
                    return new ServiceResponse<StudentAttendanceStatusDTO>(false, "record not found", new StudentAttendanceStatusDTO(), 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<StudentAttendanceStatusDTO>(false, ex.Message, new StudentAttendanceStatusDTO(), 500);
            }
        }

        public async Task<ServiceResponse<string>> SaveStudentAttendanceStatus(List<StudentAttendanceStatusDTO> request)
        {
            try
            {
                string insertSql = @"INSERT INTO [dbo].[tbl_StudentAttendanceStatus] (Student_Attendance_Status_Type, Short_Name, InstituteId)
                                     VALUES (@Student_Attendance_Status_Type, @Short_Name, @InstituteId);";
                
                string updateSql = @"UPDATE [dbo].[tbl_StudentAttendanceStatus]
                                     SET Student_Attendance_Status_Type = @Student_Attendance_Status_Type,
                                         Short_Name = @Short_Name
                                     WHERE Student_Attendance_Status_id = @Student_Attendance_Status_id;";

                foreach (var item in request)
                {
                    if (item.Student_Attendance_Status_id == 0)
                    {
                        await _connection.ExecuteAsync(insertSql, new
                        {
                            Student_Attendance_Status_Type = item.Student_Attendance_Status_Type,
                            Short_Name = item.Short_Name,
                            InstituteId = item.InstituteId
                        });
                    }
                    else
                    {
                        await _connection.ExecuteAsync(updateSql, new
                        {
                            Student_Attendance_Status_Type = item.Student_Attendance_Status_Type,
                            Short_Name = item.Short_Name,
                            Student_Attendance_Status_id = item.Student_Attendance_Status_id
                        });
                    }
                }

                return new ServiceResponse<string>(true, "Operation successful", "Data processed successfully", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteStudentAttendanceStatus(int Student_Attendance_Status_id)
        {
            try
            {


                string query1 = @"
                         SELECT COUNT(0)
                         FROM [dbo].[tbl_StudentAttendanceMaster]
                         WHERE Student_Attendance_Status_id = @Student_Attendance_Status_id"
          ;

                int count = await _connection.ExecuteScalarAsync<int>(query1, new { Student_Attendance_Status_id });

                if (count > 0)
                {
                    return new ServiceResponse<string>(false, "There is a dependency in Student Attendance, so it cannot be deleted.", string.Empty, 400);
                }
                string sql = @"update  [dbo].[tbl_StudentAttendanceStatus]
                        set isDelete = 1
                       WHERE Student_Attendance_Status_id = @Student_Attendance_Status_id";

                // Execute the query and retrieve the number of affected rows
                int affectedRows = await _connection.ExecuteAsync(sql, new { Student_Attendance_Status_id });
                if (affectedRows > 0)
                {
                    return new ServiceResponse<string>(true, "Operation successful", "Data deleted successfully", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "operation failed", string.Empty, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }
    }
}