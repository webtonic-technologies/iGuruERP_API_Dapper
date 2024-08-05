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
    public class RoomTypeRepository : IRoomTypeRepository
    {
        private readonly string _connectionString;

        public RoomTypeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> AddUpdateRoomType(AddUpdateRoomTypeRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open(); // Ensure the connection is open
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        string sqlQuery = request.RoomTypeID == 0
                            ? @"INSERT INTO tblRoomTypes (RoomType, InstituteID, IsActive) VALUES (@RoomTypeName, @InstituteID, 1); SELECT CAST(SCOPE_IDENTITY() as int)"
                            : @"UPDATE tblRoomTypes SET RoomType = @RoomTypeName, InstituteID = @InstituteID WHERE RoomTypeID = @RoomTypeID";

                        var roomTypeId = request.RoomTypeID == 0
                            ? await db.ExecuteScalarAsync<int>(sqlQuery, new { request.RoomTypeName, request.InstituteID }, transaction)
                            : await db.ExecuteAsync(sqlQuery, new { request.RoomTypeName, request.InstituteID, request.RoomTypeID }, transaction);

                        if (request.RoomTypeID == 0)
                        {
                            roomTypeId = roomTypeId;
                        }
                        else
                        {
                            roomTypeId = (int)request.RoomTypeID;
                        }

                        // Handle Room Facilities
                        await db.ExecuteAsync(@"DELETE FROM tblRoomFacilityMapping WHERE RoomTypeID = @RoomTypeID", new { RoomTypeID = roomTypeId }, transaction);
                        if (request.RoomFacilityIDs != null)
                        {
                            foreach (var facilityId in request.RoomFacilityIDs)
                            {
                                await db.ExecuteAsync(@"INSERT INTO tblRoomFacilityMapping (RoomTypeID, RoomFacilityID) VALUES (@RoomTypeID, @RoomFacilityID)", new { RoomTypeID = roomTypeId, RoomFacilityID = facilityId }, transaction);
                            }
                        }

                        // Handle Building Facilities
                        await db.ExecuteAsync(@"DELETE FROM tblBuildingFacilityMapping WHERE RoomTypeID = @RoomTypeID", new { RoomTypeID = roomTypeId }, transaction);
                        if (request.BuildingFacilityIDs != null)
                        {
                            foreach (var facilityId in request.BuildingFacilityIDs)
                            {
                                await db.ExecuteAsync(@"INSERT INTO tblBuildingFacilityMapping (RoomTypeID, BuildingFacilityID) VALUES (@RoomTypeID, @BuildingFacilityID)", new { RoomTypeID = roomTypeId, BuildingFacilityID = facilityId }, transaction);
                            }
                        }

                        // Handle Other Facilities
                        await db.ExecuteAsync(@"DELETE FROM tblOtherFacilityMapping WHERE RoomTypeID = @RoomTypeID", new { RoomTypeID = roomTypeId }, transaction);
                        if (request.OtherFacilityIDs != null)
                        {
                            foreach (var facilityId in request.OtherFacilityIDs)
                            {
                                await db.ExecuteAsync(@"INSERT INTO tblOtherFacilityMapping (RoomTypeID, OtherFacilityID) VALUES (@RoomTypeID, @OtherFacilityID)", new { RoomTypeID = roomTypeId, OtherFacilityID = facilityId }, transaction);
                            }
                        }

                        transaction.Commit();

                        return roomTypeId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }


        public async Task<PagedResponse<RoomTypeResponse>> GetAllRoomTypes(GetAllRoomTypesRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string countQuery = @"SELECT COUNT(*) FROM tblRoomTypes WHERE InstituteID = @InstituteID AND IsActive = 1";
                int totalCount = await db.ExecuteScalarAsync<int>(countQuery, new { request.InstituteID });

                string sqlQuery = @"SELECT RoomTypeID, RoomType AS RoomTypeName, InstituteID, IsActive FROM tblRoomTypes 
                                    WHERE InstituteID = @InstituteID AND IsActive = 1
                                    ORDER BY RoomType
                                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var roomTypes = await db.QueryAsync<RoomTypeResponse>(sqlQuery, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                return new PagedResponse<RoomTypeResponse>(roomTypes, request.PageNumber, request.PageSize, totalCount);
            }
        }

        public async Task<RoomTypeResponse> GetRoomTypeById(int roomTypeId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"SELECT RoomTypeID, RoomType AS RoomTypeName, InstituteID, IsActive FROM tblRoomTypes WHERE RoomTypeID = @RoomTypeID AND IsActive = 1";
                return await db.QueryFirstOrDefaultAsync<RoomTypeResponse>(sqlQuery, new { RoomTypeID = roomTypeId });
            }
        }

        public async Task<int> DeleteRoomType(int roomTypeId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"UPDATE tblRoomTypes SET IsActive = 0 WHERE RoomTypeID = @RoomTypeID";
                return await db.ExecuteAsync(sqlQuery, new { RoomTypeID = roomTypeId });
            }
        }
    }
}
