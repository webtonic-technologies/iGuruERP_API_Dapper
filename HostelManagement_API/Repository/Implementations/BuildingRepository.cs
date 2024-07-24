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
    public class BuildingRepository : IBuildingRepository
    {
        private readonly string _connectionString;

        public BuildingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> AddUpdateBuilding(AddUpdateBuildingRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = request.BuildingID == 0
                    ? @"INSERT INTO tblBuilding (BuildingName, BlockID, InstituteID, IsActive) VALUES (@BuildingName, @BlockID, @InstituteID, 1); SELECT CAST(SCOPE_IDENTITY() as int)"
                    : @"UPDATE tblBuilding SET BuildingName = @BuildingName, BlockID = @BlockID, InstituteID = @InstituteID WHERE BuildingID = @BuildingID";

                return request.BuildingID == null
                    ? await db.ExecuteScalarAsync<int>(sqlQuery, new { request.BuildingName, request.BlockID, request.InstituteID })
                    : await db.ExecuteAsync(sqlQuery, new { request.BuildingName, request.BlockID, request.InstituteID, request.BuildingID });
            }
        }

        public async Task<List<GetAllBuildingsResponse>> GetAllBuildings(GetAllBuildingsRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"SELECT b.BuildingID, b.BuildingName, bl.BlockID, bl.BlockName 
                                    FROM tblBuilding b
                                    JOIN tblBlock bl ON b.BlockID = bl.BlockID
                                    WHERE b.InstituteID = @InstituteID AND b.IsActive = 1
                                    ORDER BY bl.BlockName, b.BuildingName";

                var buildings = await db.QueryAsync<BuildingResponse>(sqlQuery, new { request.InstituteID });

                var groupedBuildings = buildings.GroupBy(b => b.BlockName)
                                                .Select(g => new GetAllBuildingsResponse
                                                {
                                                    BlockName = g.Key,
                                                    Buildings = g.Select(b => b.BuildingName).ToList()
                                                })
                                                .ToList();

                return groupedBuildings;
            }
        }

        public async Task<BuildingResponse> GetBuildingById(int buildingId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"SELECT b.BuildingID, b.BuildingName, bl.BlockID, bl.BlockName 
                                    FROM tblBuilding b
                                    JOIN tblBlock bl ON b.BlockID = bl.BlockID
                                    WHERE b.BuildingID = @BuildingID";
                return await db.QueryFirstOrDefaultAsync<BuildingResponse>(sqlQuery, new { BuildingID = buildingId });
            }
        }

        public async Task<int> DeleteBuilding(int buildingId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"UPDATE tblBuilding SET IsActive = 0 WHERE BuildingID = @BuildingID";
                return await db.ExecuteAsync(sqlQuery, new { BuildingID = buildingId });
            }
        }
    }
}
