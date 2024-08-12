using Dapper;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Lesson_API.Repository.Implementations
{
    public class LessonPlanningChapterRepository : ILessonPlanningChapterRepository
    {
        private readonly IDbConnection _dbConnection;

        public LessonPlanningChapterRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<List<LessonPlanningChapterModel>> GetChaptersByInstituteId(int instituteID)
        {
            var sql = "SELECT * FROM tblCurriculumChapter WHERE InstituteID = @InstituteID AND IsActive = 1";
            var chapters = await _dbConnection.QueryAsync<LessonPlanningChapterModel>(sql, new { InstituteID = instituteID });
            return chapters.AsList();
        }
    }
}
