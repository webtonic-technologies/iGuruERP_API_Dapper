using Dapper;
using Lesson_API.Model;
using Lesson_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Lesson_API.Repository.Implementations
{
    public class PlanTypeRepository : IPlanTypeRepository
    {
        private readonly IDbConnection _dbConnection;

        public PlanTypeRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<List<PlanType>> GetAllPlanTypesAsync()
        {
            var sql = "SELECT PlanTypeID, PlanType as PlanTypeName FROM tblPlanType";
            return (await _dbConnection.QueryAsync<PlanType>(sql)).AsList();
        }
    }
}