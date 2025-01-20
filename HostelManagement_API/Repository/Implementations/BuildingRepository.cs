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
using HostelManagement_API.DTOs.ServiceResponse;

namespace HostelManagement_API.Repository.Implementations
{
    public class BuildingRepository : IBuildingRepository
    {
        private readonly string _connectionString;

        public BuildingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ServiceResponse<string>> AddUpdateBuildings(AddUpdateBuildingsRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        // Iterate through the list of buildings in the request
                        foreach (var building in request.Buildings)
                        {
                            // Insert or update query for each building
                            string sqlQuery = building.BuildingID == 0
                                ? @"INSERT INTO tblBuilding (BuildingName, BlockID, InstituteID, IsActive) 
                            VALUES (@BuildingName, @BlockID, @InstituteID, @IsActive); 
                            SELECT CAST(SCOPE_IDENTITY() as int)"
                                : @"UPDATE tblBuilding 
                            SET BuildingName = @BuildingName, BlockID = @BlockID, InstituteID = @InstituteID, IsActive = @IsActive
                            WHERE BuildingID = @BuildingID";

                            var buildingId = building.BuildingID == 0
                                ? await db.ExecuteScalarAsync<int>(sqlQuery, new { building.BuildingName, building.BlockID, building.InstituteID, building.IsActive }, transaction)
                                : await db.ExecuteAsync(sqlQuery, new { building.BuildingName, building.BlockID, building.InstituteID, building.IsActive, building.BuildingID }, transaction);
                        }

                        transaction.Commit();

                        // Return success response
                        return new ServiceResponse<string>(
                            success: true,
                            message: "Block(s) Added/Updated Successfully",
                            data: "Success",
                            statusCode: 200,
                            totalCount: null
                        );
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetAllBuildingsResponse>>> GetAllBuildings(GetAllBuildingsRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"SELECT b.BuildingID, b.BuildingName, bl.BlockID, bl.BlockName 
                            FROM tblBuilding b
                            JOIN tblBlock bl ON b.BlockID = bl.BlockID
                            WHERE b.InstituteID = @InstituteID AND b.IsActive = 1
                            ORDER BY bl.BlockName, b.BuildingName";

                // Fetch buildings data
                var buildings = await db.QueryAsync<BuildingResponse>(sqlQuery, new { request.InstituteID });

                // Group by BlockName and create a list of Building Names per Block
                var groupedBuildings = buildings.GroupBy(b => b.BlockName)
                                                .Select(g => new GetAllBuildingsResponse
                                                {
                                                    BlockName = g.Key,
                                                    Buildings = g.Select(b => b.BuildingName).ToList()
                                                })
                                                .ToList();

                // Return response with grouped buildings
                return new ServiceResponse<IEnumerable<GetAllBuildingsResponse>>(
                    success: true,
                    message: "Blocks Retrieved Successfully",
                    data: groupedBuildings,
                    statusCode: 200,
                    groupedBuildings.Count()
                );
            }
        }


        //public async Task<IEnumerable<GetAllBuildingsResponse>> GetAllBuildings(GetAllBuildingsRequest request)
        //{
        //    using (IDbConnection db = new SqlConnection(_connectionString))
        //    {
        //        string sqlQuery = @"SELECT b.BuildingID, b.BuildingName, bl.BlockID, bl.BlockName 
        //                            FROM tblBuilding b
        //                            JOIN tblBlock bl ON b.BlockID = bl.BlockID
        //                            WHERE b.InstituteID = @InstituteID AND b.IsActive = 1
        //                            ORDER BY bl.BlockName, b.BuildingName";

        //        var buildings = await db.QueryAsync<BuildingResponse>(sqlQuery, new { request.InstituteID });

        //        var groupedBuildings = buildings.GroupBy(b => b.BlockName)
        //                                        .Select(g => new GetAllBuildingsResponse
        //                                        {
        //                                            BlockName = g.Key,
        //                                            Buildings = g.Select(b => b.BuildingName).ToList()
        //                                        })
        //                                        .ToList();

        //        //return groupedBuildings;

        //        return new ServiceResponse<IEnumerable<GetAllBuildingsResponse>>(true, "Blocks Retrieved Successfully", buildings, statusCode: 200, 0);

        //    }
        //}

        public async Task<IEnumerable<BuildingResponse>> GetAllBuildingsFetch()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"
                    SELECT 
                        BuildingID, BuildingName, BlockID, InstituteID, IsActive
                    FROM tblBuilding
                    ORDER BY BuildingName";

                var buildings = await db.QueryAsync<BuildingResponse>(sqlQuery);
                return buildings;
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
