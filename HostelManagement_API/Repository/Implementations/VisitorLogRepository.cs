using Dapper;
using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HostelManagement_API.Repository.Implementations
{
    public class VisitorLogRepository : IVisitorLogRepository
    {
        private readonly string _connectionString;

        public VisitorLogRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> AddUpdateVisitorLog(AddUpdateVisitorLogRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        string sqlQuery = request.HostelVisitorID == 0
                            ? @"INSERT INTO tblHostelVisitor (VisitorCode, VisitorName, NoOfVisitor, PhoneNo, StudentID, HostelID, RoomNo, RelationshipToStudent, DateOfVisit, Address, CheckInTime, CheckOutTime, PurposeOfVisit, UploadFile, InstituteID, IsActive) 
                                VALUES (@VisitorCode, @VisitorName, @NoOfVisitor, @PhoneNo, @StudentID, @HostelID, @RoomNo, @RelationshipToStudent, @DateOfVisit, @Address, @CheckInTime, @CheckOutTime, @PurposeOfVisit, @UploadFile, @InstituteID, @IsActive); 
                                SELECT CAST(SCOPE_IDENTITY() as int)"
                            : @"UPDATE tblHostelVisitor 
                                SET VisitorCode = @VisitorCode, VisitorName = @VisitorName, NoOfVisitor = @NoOfVisitor, PhoneNo = @PhoneNo, StudentID = @StudentID, HostelID = @HostelID, RoomNo = @RoomNo, 
                                    RelationshipToStudent = @RelationshipToStudent, DateOfVisit = @DateOfVisit, Address = @Address, CheckInTime = @CheckInTime, CheckOutTime = @CheckOutTime, 
                                    PurposeOfVisit = @PurposeOfVisit, UploadFile = @UploadFile, InstituteID = @InstituteID, IsActive = @IsActive
                                WHERE HostelVisitorID = @HostelVisitorID";

                        var hostelVisitorId = request.HostelVisitorID == 0
                            ? await db.ExecuteScalarAsync<int>(sqlQuery, new { request.VisitorCode, request.VisitorName, request.NoOfVisitor, request.PhoneNo, request.StudentID, request.HostelID, request.RoomNo, request.RelationshipToStudent, request.DateOfVisit, request.Address, request.CheckInTime, request.CheckOutTime, request.PurposeOfVisit, request.UploadFile, request.InstituteID, request.IsActive }, transaction)
                            : await db.ExecuteAsync(sqlQuery, new { request.VisitorCode, request.VisitorName, request.NoOfVisitor, request.PhoneNo, request.StudentID, request.HostelID, request.RoomNo, request.RelationshipToStudent, request.DateOfVisit, request.Address, request.CheckInTime, request.CheckOutTime, request.PurposeOfVisit, request.UploadFile, request.InstituteID, request.IsActive, request.HostelVisitorID }, transaction);

                        if (request.HostelVisitorID == 0)
                        {
                            hostelVisitorId = hostelVisitorId;
                        }
                        else
                        {
                            hostelVisitorId = (int)request.HostelVisitorID;
                        }

                        transaction.Commit();
                        return hostelVisitorId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<PagedResponse<VisitorLogResponse>> GetAllVisitorLogs(GetAllVisitorLogsRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string countQuery = @"SELECT COUNT(*) FROM tblHostelVisitor WHERE InstituteID = @InstituteID";
                int totalCount = await db.ExecuteScalarAsync<int>(countQuery, new { request.InstituteID });

                string sqlQuery = @"
                    SELECT 
                        HostelVisitorID, VisitorCode, VisitorName, NoOfVisitor, PhoneNo, StudentID, HostelID, RoomNo, RelationshipToStudent, DateOfVisit, Address, CheckInTime, CheckOutTime, PurposeOfVisit, UploadFile, InstituteID, IsActive
                    FROM tblHostelVisitor
                    WHERE InstituteID = @InstituteID
                    ORDER BY DateOfVisit
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                var visitorLogs = await db.QueryAsync<VisitorLogResponse>(sqlQuery, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                return new PagedResponse<VisitorLogResponse>(visitorLogs, request.PageNumber, request.PageSize, totalCount);
            }
        }

        public async Task<VisitorLogResponse> GetVisitorLogById(int hostelVisitorId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"
                    SELECT 
                        HostelVisitorID, VisitorCode, VisitorName, NoOfVisitor, PhoneNo, StudentID, HostelID, RoomNo, RelationshipToStudent, DateOfVisit, Address, CheckInTime, CheckOutTime, PurposeOfVisit, UploadFile, InstituteID, IsActive
                    FROM tblHostelVisitor
                    WHERE HostelVisitorID = @HostelVisitorID";

                return await db.QueryFirstOrDefaultAsync<VisitorLogResponse>(sqlQuery, new { HostelVisitorID = hostelVisitorId });
            }
        }

        public async Task<int> DeleteVisitorLog(int hostelVisitorId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"UPDATE tblHostelVisitor SET IsActive = 0 WHERE HostelVisitorID = @HostelVisitorID";
                return await db.ExecuteAsync(sqlQuery, new { HostelVisitorID = hostelVisitorId });
            }
        }
    }
}
