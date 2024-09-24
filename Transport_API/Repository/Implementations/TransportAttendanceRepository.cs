using Dapper;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;
using Transport_API.Repository.Interfaces;
using System.Data;

namespace Transport_API.Repository.Implementations
{
    public class TransportAttendanceRepository : ITransportAttendanceRepository
    {
        private readonly IDbConnection _dbConnection;

        public TransportAttendanceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<string>> AddUpdateTransportAttendance(TransportAttendance transportAttendance)
        {
            string sql;
            if (transportAttendance.TAID == 0) // Assuming TAID is the identifier for tblTransportAttendance
            {
                sql = @"INSERT INTO tblTransportAttendance (RoutePlanID, AttendanceTypeID, StudentID, AttendanceStatus, AttendanceDate, Remarks) 
                VALUES (@RoutePlanID, @AttendanceTypeID, @StudentID, @AttendanceStatus, @AttendanceDate, @Remarks)";
            }
            else
            {
                sql = @"UPDATE tblTransportAttendance 
                SET RoutePlanID = @RoutePlanID, 
                    AttendanceTypeID = @AttendanceTypeID, 
                    StudentID = @StudentID, 
                    AttendanceStatus = @AttendanceStatus, 
                    AttendanceDate = @AttendanceDate, 
                    Remarks = @Remarks 
                WHERE TAID = @TAID";
            }

            var result = await _dbConnection.ExecuteAsync(sql, transportAttendance);
            if (result > 0)
            {
                return new ServiceResponse<string>(true, "Operation Successful", "Transport attendance added/updated successfully", StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating transport attendance", StatusCodes.Status400BadRequest);
            }
        }

        public async Task<ServiceResponse<IEnumerable<TransportAttendance>>> GetAllTransportAttendance(GetTransportAttendanceRequest request)
        {
            string countSql = @"SELECT COUNT(*) FROM tbl_Transport_Attendance";
            int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql);

            string sql = @"SELECT * FROM tbl_Transport_Attendance ORDER BY TransportAttendanceId OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var transportAttendances = await _dbConnection.QueryAsync<TransportAttendance>(sql, new { Offset = (request.PageNumber - 1) * request.PageSize, PageSize = request.PageSize });

            if (transportAttendances.Any())
            {
                return new ServiceResponse<IEnumerable<TransportAttendance>>(true, "Records Found", transportAttendances, StatusCodes.Status200OK, totalCount);
            }
            else
            {
                return new ServiceResponse<IEnumerable<TransportAttendance>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<TransportAttendance>> GetTransportAttendanceById(int transportAttendanceId)
        {
            string sql = @"SELECT * FROM tbl_Transport_Attendance WHERE TransportAttendanceId = @TransportAttendanceId";
            var transportAttendance = await _dbConnection.QueryFirstOrDefaultAsync<TransportAttendance>(sql, new { TransportAttendanceId = transportAttendanceId });

            if (transportAttendance != null)
            {
                return new ServiceResponse<TransportAttendance>(true, "Record Found", transportAttendance, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<TransportAttendance>(false, "Record Not Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateTransportAttendanceStatus(int transportAttendanceId)
        {
            string sql = @"UPDATE tbl_Transport_Attendance SET IsActive = ~IsActive WHERE TransportAttendanceId = @TransportAttendanceId";
            var result = await _dbConnection.ExecuteAsync(sql, new { TransportAttendanceId = transportAttendanceId });

            if (result > 0)
            {
                return new ServiceResponse<bool>(true, "Status Updated Successfully", true, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<bool>(false, "Status Update Failed", false, StatusCodes.Status400BadRequest);
            }
        }
    }
}
