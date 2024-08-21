using Dapper;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Lesson_API.Repository.Implementations
{
    public class AssignmentTypeRepository : IAssignmentTypeRepository
    {
        private readonly IDbConnection _dbConnection;

        public AssignmentTypeRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<List<AssignmentTypeModel>> GetAllAssignmentTypes()
        {
            var sql = "SELECT * FROM tblAssignmentType";

            var result = await _dbConnection.QueryAsync<AssignmentTypeModel>(sql);
            return result.ToList();
        }
    }
}
