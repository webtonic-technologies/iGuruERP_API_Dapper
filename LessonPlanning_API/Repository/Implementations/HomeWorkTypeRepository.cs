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
    public class HomeWorkTypeRepository : IHomeWorkTypeRepository
    {
        private readonly IDbConnection _dbConnection;

        public HomeWorkTypeRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<List<HomeWorkTypeModel>> GetAllHomeWorkTypes()
        {
            var sql = "SELECT * FROM tblHomeworkType";

            var result = await _dbConnection.QueryAsync<HomeWorkTypeModel>(sql);
            return result.ToList();
        }
    }
}
