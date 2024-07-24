using Dapper;
using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Implementations
{
    public class HostelRepository : IHostelRepository
    {
        private readonly string _connectionString;

        public HostelRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> AddUpdateHostel(AddUpdateHostelRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        string sqlQuery = request.HostelID == 0
                            ? @"INSERT INTO tblHostel (HostelName, HostelTypeID, HostelPhoneNo, HostelWarden, Address, InstituteID, IsActive, Attachments) 
                                VALUES (@HostelName, @HostelTypeID, @HostelPhoneNo, @HostelWarden, @Address, @InstituteID, @IsActive, @Attachments); 
                                SELECT CAST(SCOPE_IDENTITY() as int)"
                            : @"UPDATE tblHostel 
                                SET HostelName = @HostelName, HostelTypeID = @HostelTypeID, HostelPhoneNo = @HostelPhoneNo, 
                                    HostelWarden = @HostelWarden, Address = @Address, InstituteID = @InstituteID, IsActive = @IsActive, Attachments = @Attachments 
                                WHERE HostelID = @HostelID";

                        var hostelId = request.HostelID == 0
                            ? await db.ExecuteScalarAsync<int>(sqlQuery, new { request.HostelName, request.HostelTypeID, request.HostelPhoneNo, request.HostelWarden, request.Address, request.InstituteID, request.IsActive, request.Attachments }, transaction)
                            : await db.ExecuteAsync(sqlQuery, new { request.HostelName, request.HostelTypeID, request.HostelPhoneNo, request.HostelWarden, request.Address, request.InstituteID, request.IsActive, request.Attachments, request.HostelID }, transaction);

                        if (request.HostelID == 0)
                        {
                            hostelId = hostelId;
                        }
                        else
                        {
                            hostelId = (int)request.HostelID;
                        }

                        // Handle Block Mappings
                        await db.ExecuteAsync(@"DELETE FROM tblHostelBlockMapping WHERE HostelID = @HostelID", new { HostelID = hostelId }, transaction);
                        if (request.BlockIDs != null)
                        {
                            foreach (var blockId in request.BlockIDs)
                            {
                                await db.ExecuteAsync(@"INSERT INTO tblHostelBlockMapping (HostelID, BlockID) VALUES (@HostelID, @BlockID)", new { HostelID = hostelId, BlockID = blockId }, transaction);
                            }
                        }

                        // Handle Building Mappings
                        await db.ExecuteAsync(@"DELETE FROM tblHostelBuildingMapping WHERE HostelID = @HostelID", new { HostelID = hostelId }, transaction);
                        if (request.BuildingIDs != null)
                        {
                            foreach (var buildingId in request.BuildingIDs)
                            {
                                await db.ExecuteAsync(@"INSERT INTO tblHostelBuildingMapping (HostelID, BuildingID) VALUES (@HostelID, @BuildingID)", new { HostelID = hostelId, BuildingID = buildingId }, transaction);
                            }
                        }

                        // Handle Floor Mappings
                        await db.ExecuteAsync(@"DELETE FROM tblHostelFloorMapping WHERE HostelID = @HostelID", new { HostelID = hostelId }, transaction);
                        if (request.FloorIDs != null)
                        {
                            foreach (var floorId in request.FloorIDs)
                            {
                                await db.ExecuteAsync(@"INSERT INTO tblHostelFloorMapping (HostelID, FloorID) VALUES (@HostelID, @FloorID)", new { HostelID = hostelId, FloorID = floorId }, transaction);
                            }
                        }

                        transaction.Commit();

                        return hostelId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public async Task<PagedResponse<HostelResponse>> GetAllHostels(GetAllHostelsRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string countQuery = @"SELECT COUNT(*) FROM tblHostel WHERE InstituteID = @InstituteID";
                int totalCount = await db.ExecuteScalarAsync<int>(countQuery, new { request.InstituteID });

                string sqlQuery = @"
                    SELECT 
                        h.HostelID, h.HostelName, h.HostelTypeID, ht.HostelType, h.Address, h.HostelPhoneNo AS PhoneNo, 
                        h.HostelWarden, e.First_Name, b.BlockName AS Block, 
                        (SELECT STRING_AGG(bl.BuildingName, ', ') 
                         FROM tblHostelBuildingMapping hbm2 
                         JOIN tblBuilding bl ON hbm2.BuildingID = bl.BuildingID 
                         WHERE hbm2.HostelID = h.HostelID) AS Building,
                        (SELECT STRING_AGG(f.FloorName + ' – ' + bl.BuildingName, ', ') 
                         FROM tblHostelFloorMapping hfm 
                         JOIN tblBuildingFloors f ON hfm.FloorID = f.FloorID 
                         JOIN tblBuilding bl ON f.BuildingID = bl.BuildingID
                         WHERE hfm.HostelID = h.HostelID) AS Floors
                    FROM tblHostel h
                    LEFT JOIN tblHostelType ht ON h.HostelTypeID = ht.HostelTypeID
                    LEFT JOIN tblHostelBlockMapping hbm ON h.HostelID = hbm.HostelID
                    LEFT JOIN tblBlock b ON hbm.BlockID = b.BlockID
                    LEFT JOIN tbl_EmployeeProfileMaster e ON h.HostelWarden = e.Employee_id
                    WHERE h.InstituteID = 1
                    GROUP BY h.HostelID, h.HostelName, h.HostelTypeID, ht.HostelType, h.Address, h.HostelPhoneNo, h.HostelWarden, e.First_Name, b.BlockName
                    ORDER BY h.HostelName
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var hostels = await db.QueryAsync<HostelResponse>(sqlQuery, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                return new PagedResponse<HostelResponse>(hostels, request.PageNumber, request.PageSize, totalCount);
            }
        }
        public async Task<HostelResponse> GetHostelById(int hostelId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"SELECT * FROM tblHostel WHERE HostelID = @HostelID";
                return await db.QueryFirstOrDefaultAsync<HostelResponse>(sqlQuery, new { HostelID = hostelId });
            }
        }

        public async Task<int> DeleteHostel(int hostelId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"UPDATE tblHostel SET IsActive = 0 WHERE HostelID = @HostelID";
                return await db.ExecuteAsync(sqlQuery, new { HostelID = hostelId });
            }
        }
    }
}
