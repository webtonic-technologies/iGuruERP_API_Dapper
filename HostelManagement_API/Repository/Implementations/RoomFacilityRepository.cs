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
    public class RoomFacilityRepository : IRoomFacilityRepository
    {
        private readonly string _connectionString;

        public RoomFacilityRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<FacilityResponse>> GetAllRoomFacilities()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = "SELECT RoomFacilityID AS FacilityID, RoomFacility AS FacilityName FROM tblRoomFacilities";
                return await db.QueryAsync<FacilityResponse>(sqlQuery);
            }
        }
    }
}
