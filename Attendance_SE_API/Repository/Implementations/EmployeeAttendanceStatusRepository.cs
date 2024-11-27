using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.ServiceResponse;
using Dapper;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attendance_SE_API.Models;
using Attendance_SE_API.Repository.Interfaces;

namespace Attendance_SE_API.Repository.Implementations
{
    public class EmployeeAttendanceStatusRepository : IEmployeeAttendanceStatusRepository
    {
        private readonly IDbConnection _connection;

        public EmployeeAttendanceStatusRepository(IDbConnection connection)
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
                    string query = @"INSERT INTO tblEmployeeAttendanceStatus (StatusName, ShortName, IsActive, InstituteID) 
                             VALUES (@StatusName, @ShortName, @IsActive, @InstituteID)";
                    await _connection.ExecuteAsync(query, request);
                    return new ServiceResponse<string>(true, "Attendance status added successfully.", null, 201);
                }
                else
                {
                    // Update existing status
                    string query = @"UPDATE tblEmployeeAttendanceStatus SET StatusName = @StatusName, ShortName = @ShortName, 
                             IsActive = @IsActive, InstituteID = @InstituteID 
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


        //public async Task<ServiceResponse<List<AttendanceStatus>>> GetAllAttendanceStatuses(GetAllAttendanceStatusRequest request)
        //{
        //    try
        //    {
        //        string query = "SELECT * FROM tblEmployeeAttendanceStatus WHERE IsActive = 1";
        //        var statuses = await _connection.QueryAsync<AttendanceStatus>(query);

        //        var paginatedList = statuses.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
        //        return new ServiceResponse<List<AttendanceStatus>>(true, "Records found", paginatedList, 200, statuses.Count());
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<List<AttendanceStatus>>(false, ex.Message, null, 500);
        //    }
        //}

        public async Task<ServiceResponse<List<AttendanceStatus>>> GetAllAttendanceStatuses(GetAllAttendanceStatusRequest request)
        {
            try
            {
                // Start query to get records with IsDefault = 1
                string query = "SELECT * FROM tblEmployeeAttendanceStatus WHERE IsActive = 1 AND IsDefault = 1";

                // Add condition for IsDefault = 0 and InstituteID
                query += " UNION ALL SELECT * FROM tblEmployeeAttendanceStatus WHERE IsActive = 1 AND IsDefault = 0 AND InstituteID = @InstituteID";

                // Execute the query
                var statuses = await _connection.QueryAsync<AttendanceStatus>(query, new { InstituteID = request.InstituteID });

                // Pagination logic
                var paginatedList = statuses
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                return new ServiceResponse<List<AttendanceStatus>>(true, "Records found", paginatedList, 200, statuses.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AttendanceStatus>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<AttendanceStatus>>> GetAllAttendanceStatusesDDL(GetAllAttendanceStatusDDLRequest request)
        {
            try
            {
                // Query to get records with IsDefault = 1 and IsActive = 1
                string query = "SELECT * FROM tblEmployeeAttendanceStatus WHERE IsActive = 1 AND IsDefault = 1";

                // Add condition for IsDefault = 0 and InstituteID
                query += " UNION ALL SELECT * FROM tblEmployeeAttendanceStatus WHERE IsActive = 1 AND IsDefault = 0 AND InstituteID = @InstituteID";

                // Execute the query
                var statuses = await _connection.QueryAsync<AttendanceStatus>(query, new { InstituteID = request.InstituteID });

                return new ServiceResponse<List<AttendanceStatus>>(true, "Records found", statuses.AsList(), 200);
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
                string query = "SELECT * FROM tblEmployeeAttendanceStatus WHERE StatusID = @StatusID AND IsActive = 1";
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
                string checkQuery = "SELECT COUNT(*) FROM tblEmployeeAttendanceStatus WHERE StatusID = @StatusID AND IsDefault = 1";
                int isDefaultCount = await _connection.ExecuteScalarAsync<int>(checkQuery, new { StatusID = statusId });

                if (isDefaultCount > 0)
                {
                    return new ServiceResponse<bool>(false, "Cannot delete a status that is marked as default.", false, 400);
                }

                // If not default, proceed to mark as inactive
                string query = "UPDATE tblEmployeeAttendanceStatus SET IsActive = 0 WHERE StatusID = @StatusID";
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
