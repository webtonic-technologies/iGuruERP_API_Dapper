using Dapper;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Implementations
{
    public class BuildingFacilityRepository : IBuildingFacilityRepository
    {
        private readonly string _connectionString;

        public BuildingFacilityRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<FacilityResponse>> GetAllBuildingFacilities()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = "SELECT BuildingFacilityID AS FacilityID, BuildingFacility AS FacilityName FROM tblBuildingFacilities";
                return await db.QueryAsync<FacilityResponse>(sqlQuery);
            }
        }
    }
}
