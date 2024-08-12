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
    public class LessonPlanningSubtopicRepository : ILessonPlanningSubtopicRepository
    {
        private readonly IDbConnection _dbConnection;

        public LessonPlanningSubtopicRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<List<LessonPlanningSubtopicModel>> GetSubtopicsByInstituteId(int instituteID)
        {
            var sql = @"
                SELECT * FROM tblCurriculumSubTopic 
                WHERE InstituteID = @InstituteID AND IsActive = 1";

            var result = await _dbConnection.QueryAsync<LessonPlanningSubtopicModel>(sql, new { InstituteID = instituteID });
            return result.ToList();
        }
    }
}
