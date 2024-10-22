using Attendance_SE_API.Models;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using Attendance_SE_API.DTOs.Requests;
using Dapper;
using System.Data;


namespace Attendance_SE_API.Repository.Implementations
{
    public class StudentAttendanceStatusRepository : IStudentAttendanceStatusRepository
    {
        private readonly IDbConnection _connection;

        public StudentAttendanceStatusRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<string>> AddUpdateAttendanceStatus(AttendanceStatus request)
        {
            try
            {
                if (request.StatusID == 0)
                {
                    // Insert new status
                    string query = @"INSERT INTO tblStudentAttendanceStatus (StatusName, ShortName, IsDefault, IsActive, InstituteID) 
                                     VALUES (@StatusName, @ShortName, @IsDefault, @IsActive, @InstituteID)";
                    await _connection.ExecuteAsync(query, request);
                    return new ServiceResponse<string>(true, "Attendance status added successfully.", null, 201);
                }
                else
                {
                    // Update existing status
                    string query = @"UPDATE tblStudentAttendanceStatus SET StatusName = @StatusName, ShortName = @ShortName, 
                                     IsDefault = @IsDefault, IsActive = @IsActive, InstituteID = @InstituteID 
                                     WHERE StatusID = @StatusID";
                    await _connection.ExecuteAsync(query, request);
                    return new ServiceResponse<string>(true, "Attendance status updated successfully.", null, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<AttendanceStatus>>> GetAllAttendanceStatuses(GetAllAttendanceStatusRequest request)
        {
            try
            {
                string query = "SELECT * FROM tblStudentAttendanceStatus WHERE IsActive = 1";
                var statuses = await _connection.QueryAsync<AttendanceStatus>(query);

                var paginatedList = statuses.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                return new ServiceResponse<List<AttendanceStatus>>(true, "Records found", paginatedList, 200, statuses.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AttendanceStatus>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<AttendanceStatus>> GetAttendanceStatusById(int statusId)
        {
            try
            {
                string query = "SELECT * FROM tblStudentAttendanceStatus WHERE StatusID = @StatusID";
                var status = await _connection.QueryFirstOrDefaultAsync<AttendanceStatus>(query, new { StatusID = statusId });

                if (status != null)
                {
                    return new ServiceResponse<AttendanceStatus>(true, "Record found", status, 200);
                }
                return new ServiceResponse<AttendanceStatus>(false, "Record not found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AttendanceStatus>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteStatus(int statusId)
        {
            try
            {
                // Check if the status is marked as default
                string checkQuery = "SELECT COUNT(*) FROM tblStudentAttendanceStatus WHERE StatusID = @StatusID AND IsDefault = 1";
                int isDefaultCount = await _connection.ExecuteScalarAsync<int>(checkQuery, new { StatusID = statusId });

                if (isDefaultCount > 0)
                {
                    return new ServiceResponse<bool>(false, "Cannot delete a status that is marked as default.", false, 400);
                }

                // If not default, proceed to mark as inactive
                string query = "UPDATE tblStudentAttendanceStatus SET IsActive = 0 WHERE StatusID = @StatusID";
                await _connection.ExecuteAsync(query, new { StatusID = statusId });

                return new ServiceResponse<bool>(true, "Status deleted successfully.", true, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

    }
}
