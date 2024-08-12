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
    public class OtherFacilityRepository : IOtherFacilityRepository
    {
        private readonly string _connectionString;

        public OtherFacilityRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<FacilityResponse>> GetAllOtherFacilities()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = "SELECT OtherFacilityID AS FacilityID, OtherFacility AS FacilityName FROM tblOtherFacilities";
                return await db.QueryAsync<FacilityResponse>(sqlQuery);
            }
        }
    }
}
