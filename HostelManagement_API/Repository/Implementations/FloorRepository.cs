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
    public class FloorRepository : IFloorRepository
    {
        private readonly string _connectionString;

        public FloorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        } 

        public async Task<ServiceResponse<string>> AddUpdateFloors(AddUpdateFloorsRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        foreach (var floor in request.Floors)
                        {
                            string sqlQuery = floor.FloorID == 0
                                ? @"INSERT INTO tblBuildingFloors (FloorName, BuildingID, InstituteID, IsActive) 
                            VALUES (@FloorName, @BuildingID, @InstituteID, @IsActive); 
                            SELECT CAST(SCOPE_IDENTITY() as int)"
                                : @"UPDATE tblBuildingFloors 
                            SET FloorName = @FloorName, BuildingID = @BuildingID, InstituteID = @InstituteID, IsActive = @IsActive
                            WHERE FloorID = @FloorID";

                            var floorId = floor.FloorID == 0
                                ? await db.ExecuteScalarAsync<int>(sqlQuery, new { floor.FloorName, floor.BuildingID, floor.InstituteID, floor.IsActive }, transaction)
                                : await db.ExecuteAsync(sqlQuery, new { floor.FloorName, floor.BuildingID, floor.InstituteID, floor.IsActive, floor.FloorID }, transaction);
                        }

                        // Commit the transaction if successful
                        transaction.Commit();

                        // Return the success response in the required format
                        return new ServiceResponse<string>(
                            success: true,
                            message: "Floor(s) Added/Updated Successfully",
                            data: "Success",
                            statusCode: 200,
                            totalCount: null
                        );
                    }
                    catch
                    {
                        // Rollback transaction in case of error
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }


        public async Task<ServiceResponse<IEnumerable<GetAllFloorsResponse>>> GetAllFloors(GetAllFloorsRequest request)
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

                //return groupedFloors;
                return new ServiceResponse<IEnumerable<GetAllFloorsResponse>>(
                    success: true,
                    message: "Floors Retrieved Successfully",
                    data: groupedFloors,
                    statusCode: 200,
                    groupedFloors.Count()
                );
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
