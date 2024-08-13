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
    public class ChapterSubtopicListRepository : IChapterSubtopicListRepository
    {
        private readonly IDbConnection _dbConnection;

        public ChapterSubtopicListRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<List<ChapterSubtopicList>> GetChapterSubtopics(int curriculumID, int instituteID)
        {
            var sql = @"
                SELECT * FROM tblCurriculumChapter WHERE CurriculumID = @CurriculumID AND InstituteID = @InstituteID;
                SELECT * FROM tblCurriculumSubTopic WHERE CurriculumChapterID IN 
                    (SELECT CurriculumChapterID FROM tblCurriculumChapter WHERE CurriculumID = @CurriculumID AND InstituteID = @InstituteID);";

            using (var multi = await _dbConnection.QueryMultipleAsync(sql, new { CurriculumID = curriculumID, InstituteID = instituteID }))
            {
                var chapters = await multi.ReadAsync<ChapterSubtopicList>();
                var subtopics = await multi.ReadAsync<CurriculumSubTopic>();

                foreach (var chapter in chapters)
                {
                    chapter.CurriculumSubTopics = subtopics.Where(st => st.CurriculumChapterID == chapter.CurriculumChapterID).ToList();
                }

                return chapters.ToList();
            }
        }
    }
}
