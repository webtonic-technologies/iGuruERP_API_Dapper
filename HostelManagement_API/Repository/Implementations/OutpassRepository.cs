using Dapper;
using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Implementations
{
    public class OutpassRepository : IOutpassRepository
    {
        private readonly string _connectionString;

        public OutpassRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> AddUpdateOutpass(AddUpdateOutpassRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        string sqlQuery = request.OutpassID == 0
                            ? @"INSERT INTO tblHostelOutpass (OutpassCode, OutpassDate, HostelID, StudentID, RoomNo, DepartureTime, ExpectedArrivalTime, EntryTime, Reason, Remarks, UploadFile, InstituteID, IsActive) 
                                VALUES (@OutpassCode, @OutpassDate, @HostelID, @StudentID, @RoomNo, @DepartureTime, @ExpectedArrivalTime, @EntryTime, @Reason, @Remarks, @UploadFile, @InstituteID, @IsActive); 
                                SELECT CAST(SCOPE_IDENTITY() as int)"
                            : @"UPDATE tblHostelOutpass 
                                SET OutpassCode = @OutpassCode, OutpassDate = @OutpassDate, HostelID = @HostelID, StudentID = @StudentID, 
                                    RoomNo = @RoomNo, DepartureTime = @DepartureTime, ExpectedArrivalTime = @ExpectedArrivalTime, 
                                    EntryTime = @EntryTime, Reason = @Reason, Remarks = @Remarks, UploadFile = @UploadFile, 
                                    InstituteID = @InstituteID, IsActive = @IsActive
                                WHERE OutpassID = @OutpassID";

                        var outpassId = request.OutpassID == 0
                            ? await db.ExecuteScalarAsync<int>(sqlQuery, new { request.OutpassCode, request.OutpassDate, request.HostelID, request.StudentID, request.RoomNo, request.DepartureTime, request.ExpectedArrivalTime, request.EntryTime, request.Reason, request.Remarks, request.UploadFile, request.InstituteID, request.IsActive }, transaction)
                            : await db.ExecuteAsync(sqlQuery, new { request.OutpassCode, request.OutpassDate, request.HostelID, request.StudentID, request.RoomNo, request.DepartureTime, request.ExpectedArrivalTime, request.EntryTime, request.Reason, request.Remarks, request.UploadFile, request.InstituteID, request.IsActive, request.OutpassID }, transaction);

                        if (request.OutpassID == 0)
                        {
                            outpassId = outpassId;
                        }
                        else
                        {
                            outpassId = (int)request.OutpassID;
                        }

                        transaction.Commit();
                        return outpassId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<PagedResponse<OutpassResponse>> GetAllOutpass(GetAllOutpassRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string countQuery = @"SELECT COUNT(*) FROM tblHostelOutpass WHERE InstituteID = @InstituteID";
                int totalCount = await db.ExecuteScalarAsync<int>(countQuery, new { request.InstituteID });

                string sqlQuery = @"
                    SELECT 
                        OutpassID, OutpassCode, OutpassDate, HostelID, StudentID, RoomNo, DepartureTime, ExpectedArrivalTime, EntryTime, Reason, Remarks, UploadFile, InstituteID, IsActive
                    FROM tblHostelOutpass
                    WHERE InstituteID = @InstituteID
                    ORDER BY OutpassDate
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                var outpasses = await db.QueryAsync<OutpassResponse>(sqlQuery, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                return new PagedResponse<OutpassResponse>(outpasses, request.PageNumber, request.PageSize, totalCount);
            }
        }

        public async Task<OutpassResponse> GetOutpassById(int outpassId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"
                    SELECT 
                        OutpassID, OutpassCode, OutpassDate, HostelID, StudentID, RoomNo, DepartureTime, ExpectedArrivalTime, EntryTime, Reason, Remarks, UploadFile, InstituteID, IsActive
                    FROM tblHostelOutpass
                    WHERE OutpassID = @OutpassID";

                return await db.QueryFirstOrDefaultAsync<OutpassResponse>(sqlQuery, new { OutpassID = outpassId });
            }
        }

        public async Task<int> DeleteOutpass(int outpassId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"UPDATE tblHostelOutpass SET IsActive = 0 WHERE OutpassID = @OutpassID";
                return await db.ExecuteAsync(sqlQuery, new { OutpassID = outpassId });
            }
        }
    }
}
