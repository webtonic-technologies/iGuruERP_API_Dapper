using Dapper;
using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Implementations
{
    public class RoomRepository : IRoomRepository
    {
        private readonly string _connectionString;

        public RoomRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> AddUpdateRoom(AddUpdateRoomRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        string sqlQuery = request.RoomID == 0
                            ? @"INSERT INTO tblRoom (RoomName, HostelID, FloorID, RoomTypeID, InstituteID, IsActive) 
                                VALUES (@RoomName, @HostelID, @FloorID, @RoomTypeID, @InstituteID, @IsActive); 
                                SELECT CAST(SCOPE_IDENTITY() as int)"
                            : @"UPDATE tblRoom 
                                SET RoomName = @RoomName, HostelID = @HostelID, FloorID = @FloorID, 
                                    RoomTypeID = @RoomTypeID, InstituteID = @InstituteID, IsActive = @IsActive
                                WHERE RoomID = @RoomID";

                        var roomId = request.RoomID == 0
                            ? await db.ExecuteScalarAsync<int>(sqlQuery, new { request.RoomName, request.HostelID, request.FloorID, request.RoomTypeID, request.InstituteID, request.IsActive }, transaction)
                            : await db.ExecuteAsync(sqlQuery, new { request.RoomName, request.HostelID, request.FloorID, request.RoomTypeID, request.InstituteID, request.IsActive, request.RoomID }, transaction);

                        if (request.RoomID == 0)
                        {
                            roomId = roomId;
                        }
                        else
                        {
                            roomId = (int)request.RoomID;
                        }

                        // Handle Room Beds
                        await db.ExecuteAsync(@"DELETE FROM tblRoomBeds WHERE RoomID = @RoomID", new { RoomID = roomId }, transaction);
                        foreach (var bed in request.RoomBeds)
                        {
                            string bedQuery = @"INSERT INTO tblRoomBeds (RoomBedName, BedPosition, RoomID, InstituteID) 
                                                VALUES (@RoomBedName, @BedPosition, @RoomID, @InstituteID)";
                            await db.ExecuteAsync(bedQuery, new { bed.RoomBedName, bed.BedPosition, RoomID = roomId, request.InstituteID }, transaction);
                        }

                        transaction.Commit();
                        return roomId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<PagedResponse<RoomResponse>> GetAllRooms(GetAllRoomsRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string countQuery = @"SELECT COUNT(*) FROM tblRoom WHERE IsActive = 1 and InstituteID = @InstituteID";
                int totalCount = await db.ExecuteScalarAsync<int>(countQuery, new { request.InstituteID });

                string sqlQuery = @"
                    SELECT 
                        r.RoomID, r.RoomName, r.RoomTypeID, rt.RoomType AS RoomTypeName, 
                        r.HostelID, h.HostelName, r.FloorID, f.FloorName, r.IsActive
                    FROM tblRoom r
                    LEFT JOIN tblRoomType rt ON r.RoomTypeID = rt.RoomTypeID
                    LEFT JOIN tblHostel h ON r.HostelID = h.HostelID
                    LEFT JOIN tblBuildingFloors f ON r.FloorID = f.FloorID
                    WHERE r.InstituteID = @InstituteID and r.IsActive = 1
                    ORDER BY r.RoomName
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                var rooms = await db.QueryAsync<RoomResponse>(sqlQuery, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                return new PagedResponse<RoomResponse>(rooms, request.PageNumber, request.PageSize, totalCount);
            }
        }

        public async Task<RoomResponse> GetRoomById(int roomId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"
                    SELECT 
                        r.RoomID, r.RoomName, r.RoomTypeID, rt.RoomType AS RoomTypeName, 
                        r.HostelID, h.HostelName, r.FloorID, f.FloorName, r.IsActive
                    FROM tblRoom r
                    LEFT JOIN tblRoomType rt ON r.RoomTypeID = rt.RoomTypeID
                    LEFT JOIN tblHostel h ON r.HostelID = h.HostelID
                    LEFT JOIN tblBuildingFloors f ON r.FloorID = f.FloorID
                    WHERE r.RoomID = @RoomID and r.IsActive = 1";

                return await db.QueryFirstOrDefaultAsync<RoomResponse>(sqlQuery, new { RoomID = roomId });
            }
        }

        public async Task<int> DeleteRoom(int roomId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"UPDATE tblRoom SET IsActive = 0 WHERE RoomID = @RoomID";
                return await db.ExecuteAsync(sqlQuery, new { RoomID = roomId });
            }
        }
    }
}
