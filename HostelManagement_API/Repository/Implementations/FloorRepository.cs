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
    public class FloorRepository : IFloorRepository
    {
        private readonly string _connectionString;

        public FloorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> AddUpdateFloor(AddUpdateFloorRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = request.FloorID == 0
                    ? @"INSERT INTO tblBuildingFloors (FloorName, BuildingID, InstituteID, IsActive) VALUES (@FloorName, @BuildingID, @InstituteID, 1); SELECT CAST(SCOPE_IDENTITY() as int)"
                    : @"UPDATE tblBuildingFloors SET FloorName = @FloorName, BuildingID = @BuildingID, InstituteID = @InstituteID WHERE FloorID = @FloorID";

                return request.FloorID == null
                    ? await db.ExecuteScalarAsync<int>(sqlQuery, new { request.FloorName, request.BuildingID, request.InstituteID })
                    : await db.ExecuteAsync(sqlQuery, new { request.FloorName, request.BuildingID, request.InstituteID, request.FloorID });
            }
        }

        public async Task<List<GetAllFloorsResponse>> GetAllFloors(GetAllFloorsRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"SELECT f.FloorID, f.FloorName, b.BuildingID, b.BuildingName, bl.BlockName 
                                    FROM tblBuildingFloors f
                                    JOIN tblBuilding b ON f.BuildingID = b.BuildingID
                                    JOIN tblBlock bl ON b.BlockID = bl.BlockID
                                    WHERE f.InstituteID = @InstituteID AND f.IsActive = 1
                                    ORDER BY bl.BlockName, b.BuildingName, f.FloorName";

                var floors = await db.QueryAsync<FloorResponse>(sqlQuery, new { request.InstituteID });

                var groupedFloors = floors.GroupBy(f => $"{f.BlockName} - {f.BuildingName}")
                                          .Select(g => new GetAllFloorsResponse
                                          {
                                              BuildingName = g.Key,
                                              Floors = g.Select(f => f.FloorName).ToList()
                                          })
                                          .ToList();

                return groupedFloors;
            }
        }

        public async Task<FloorResponse> GetFloorById(int floorId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"SELECT f.FloorID, f.FloorName, b.BuildingID, b.BuildingName, bl.BlockName 
                                    FROM tblBuildingFloors f
                                    JOIN tblBuilding b ON f.BuildingID = b.BuildingID
                                    JOIN tblBlock bl ON b.BlockID = bl.BlockID
                                    WHERE f.FloorID = @FloorID";
                return await db.QueryFirstOrDefaultAsync<FloorResponse>(sqlQuery, new { FloorID = floorId });
            }
        }

        public async Task<int> DeleteFloor(int floorId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"UPDATE tblBuildingFloors SET IsActive = 0 WHERE FloorID = @FloorID";
                return await db.ExecuteAsync(sqlQuery, new { FloorID = floorId });
            }
        }
    }
}
