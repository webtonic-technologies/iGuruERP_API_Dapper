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
    public class HostelTypeRepository : IHostelTypeRepository
    {
        private readonly string _connectionString;

        public HostelTypeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<HostelTypeResponse>> GetAllHostelTypes()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = "SELECT HostelTypeID, HostelType FROM tblHostelType";
                return await db.QueryAsync<HostelTypeResponse>(sqlQuery);
            }
        }
    }
}
