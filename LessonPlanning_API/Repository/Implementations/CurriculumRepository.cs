using Dapper;
using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Lesson_API.Repository.Implementations
{
    public class CurriculumRepository : ICurriculumRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<CurriculumRepository> _logger;

        public CurriculumRepository(IConfiguration configuration, ILogger<CurriculumRepository> logger)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            _logger = logger;
        }

        public async Task<ServiceResponse<string>> AddUpdateCurriculum(CurriculumRequest request)
        {
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.Open();
            }

            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                // Insert or Update Curriculum
                if (request.CurriculumID == 0)
                {
                    var curriculumSql = @"INSERT INTO tblCurriculum (ClassID, SubjectID, InstituteID, IsActive) 
                                          VALUES (@ClassID, @SubjectID, @InstituteID, 1);
                                          SELECT CAST(SCOPE_IDENTITY() as int);";
                    request.CurriculumID = await _dbConnection.QuerySingleAsync<int>(curriculumSql, new
                    {
                        request.ClassID,
                        request.SubjectID,
                        request.InstituteID
                    }, transaction);
                }
                else
                {
                    var curriculumSql = @"UPDATE tblCurriculum SET 
                                          ClassID = @ClassID, 
                                          SubjectID = @SubjectID, 
                                          InstituteID = @InstituteID
                                          WHERE CurriculumID = @CurriculumID";
                    await _dbConnection.ExecuteAsync(curriculumSql, new
                    {
                        request.CurriculumID,
                        request.ClassID,
                        request.SubjectID,
                        request.InstituteID
                    }, transaction);
                }

                // Insert or Update Curriculum Chapters
                foreach (var chapter in request.CurriculumChapters)
                {
                    if (chapter.CurriculumChapterID == 0)
                    {
                        var chapterSql = @"INSERT INTO tblCurriculumChapter (ChapterName, TotalSessions, Attachment, CurriculumID, InstituteID, IsActive) 
                                           VALUES (@ChapterName, @TotalSessions, @Attachment, @CurriculumID, @InstituteID, 1);
                                           SELECT CAST(SCOPE_IDENTITY() as int);";
                        chapter.CurriculumChapterID = await _dbConnection.QuerySingleAsync<int>(chapterSql, new
                        {
                            chapter.ChapterName,
                            chapter.TotalSessions,
                            chapter.Attachment,
                            CurriculumID = request.CurriculumID,
                            request.InstituteID
                        }, transaction);
                    }
                    else
                    {
                        var chapterSql = @"UPDATE tblCurriculumChapter SET 
                                           ChapterName = @ChapterName, 
                                           TotalSessions = @TotalSessions, 
                                           Attachment = @Attachment
                                           WHERE CurriculumChapterID = @CurriculumChapterID";
                        await _dbConnection.ExecuteAsync(chapterSql, new
                        {
                            chapter.ChapterName,
                            chapter.TotalSessions,
                            chapter.Attachment,
                            chapter.CurriculumChapterID
                        }, transaction);
                    }

                    // Insert or Update Curriculum SubTopics
                    foreach (var subTopic in chapter.CurriculumSubTopics)
                    {
                        if (subTopic.CurriculumSubTopicID == 0)
                        {
                            var subTopicSql = @"INSERT INTO tblCurriculumSubTopic (SubTopicName, TotalSession, Attachment, CurriculumChapterID, InstituteID, IsActive) 
                                                VALUES (@SubTopicName, @TotalSession, @Attachment, @CurriculumChapterID, @InstituteID, 1);
                                                SELECT CAST(SCOPE_IDENTITY() as int);";
                            subTopic.CurriculumSubTopicID = await _dbConnection.QuerySingleAsync<int>(subTopicSql, new
                            {
                                subTopic.SubTopicName,
                                subTopic.TotalSession,
                                subTopic.Attachment,
                                CurriculumChapterID = chapter.CurriculumChapterID,
                                request.InstituteID
                            }, transaction);
                        }
                        else
                        {
                            var subTopicSql = @"UPDATE tblCurriculumSubTopic SET 
                                                SubTopicName = @SubTopicName, 
                                                TotalSession = @TotalSession, 
                                                Attachment = @Attachment
                                                WHERE CurriculumSubTopicID = @CurriculumSubTopicID";
                            await _dbConnection.ExecuteAsync(subTopicSql, new
                            {
                                subTopic.SubTopicName,
                                subTopic.TotalSession,
                                subTopic.Attachment,
                                subTopic.CurriculumSubTopicID
                            }, transaction);
                        }

                        // Insert or Update Curriculum Resource Details
                        foreach (var resourceDetail in subTopic.CurriculumResourceDetails)
                        {
                            if (resourceDetail.CurriculumResourceID == 0)
                            {
                                var resourceDetailSql = @"INSERT INTO tblCurriculumResourceDetails (LearningObjectives, SuggestedActivity, TeachingResouces, TeachingMethod, Criteria, CurriculumSubTopicID, IsActive) 
                                                          VALUES (@LearningObjectives, @SuggestedActivity, @TeachingResouces, @TeachingMethod, @Criteria, @CurriculumSubTopicID, 1);
                                                          SELECT CAST(SCOPE_IDENTITY() as int);";
                                resourceDetail.CurriculumResourceID = await _dbConnection.QuerySingleAsync<int>(resourceDetailSql, new
                                {
                                    resourceDetail.LearningObjectives,
                                    resourceDetail.SuggestedActivity,
                                    resourceDetail.TeachingResouces,
                                    resourceDetail.TeachingMethod,
                                    resourceDetail.Criteria,
                                    CurriculumSubTopicID = subTopic.CurriculumSubTopicID
                                }, transaction);
                            }
                            else
                            {
                                var resourceDetailSql = @"UPDATE tblCurriculumResourceDetails SET 
                                                          LearningObjectives = @LearningObjectives, 
                                                          SuggestedActivity = @SuggestedActivity, 
                                                          TeachingResouces = @TeachingResouces, 
                                                          TeachingMethod = @TeachingMethod, 
                                                          Criteria = @Criteria
                                                          WHERE CurriculumResourceID = @CurriculumResourceID";
                                await _dbConnection.ExecuteAsync(resourceDetailSql, new
                                {
                                    resourceDetail.LearningObjectives,
                                    resourceDetail.SuggestedActivity,
                                    resourceDetail.TeachingResouces,
                                    resourceDetail.TeachingMethod,
                                    resourceDetail.Criteria,
                                    resourceDetail.CurriculumResourceID
                                }, transaction);
                            }
                        }
                    }
                }

                transaction.Commit();
                return new ServiceResponse<string>(request.CurriculumID.ToString(), true, "Curriculum added/updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding/updating curriculum.");
                transaction.Rollback();
                return new ServiceResponse<string>(null, false, "Operation failed: " + ex.Message);
            }
        }

        public async Task<ServiceResponse<List<GetAllCurriculumResponse>>> GetAllCurriculum(GetAllCurriculumRequest request)
        {
            try
            {
                var sql = @"
                    WITH CurriculumData AS (
                        SELECT 
                            s.subject_name + ' {' + STRING_AGG(c.class_name, ', ') + '}' AS Subject,
                            COUNT(ch.CurriculumChapterID) AS NumberOfChapters,
                            ROW_NUMBER() OVER (ORDER BY s.subject_name) AS RowNum
                        FROM tblCurriculum cu
                        INNER JOIN tbl_InstituteSubjects s ON cu.SubjectID = s.institute_subject_id
                        INNER JOIN tbl_Class c ON cu.ClassID = c.class_id
                        LEFT JOIN tblCurriculumChapter ch ON cu.CurriculumID = ch.CurriculumID
                        WHERE cu.InstituteID = @InstituteID AND cu.IsActive = 1
                        GROUP BY s.subject_name
                    )
                    SELECT Subject, NumberOfChapters
                    FROM CurriculumData
                    WHERE RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize";

                var result = await _dbConnection.QueryAsync<GetAllCurriculumResponse>(sql, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    request.PageSize
                });

                return new ServiceResponse<List<GetAllCurriculumResponse>>(result.ToList(), true, "Curriculums retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving curriculums.");
                return new ServiceResponse<List<GetAllCurriculumResponse>>(null, false, "Operation failed: " + ex.Message);
            }
        }

        public async Task<ServiceResponse<Curriculum>> GetCurriculumById(int id)
        {
            try
            {
                var sql = @"SELECT * FROM tblCurriculum WHERE CurriculumID = @CurriculumID AND IsActive = 1";
                var curriculum = await _dbConnection.QueryFirstOrDefaultAsync<Curriculum>(sql, new { CurriculumID = id });

                if (curriculum != null)
                {
                    return new ServiceResponse<Curriculum>(curriculum, true, "Curriculum found.");
                }
                else
                {
                    return new ServiceResponse<Curriculum>(null, false, "Curriculum not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving curriculum by ID.");
                return new ServiceResponse<Curriculum>(null, false, "Operation failed: " + ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteCurriculum(int id)
        {
            try
            {
                var sql = @"UPDATE tblCurriculum SET IsActive = 0 WHERE CurriculumID = @CurriculumID";
                var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { CurriculumID = id });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, true, "Curriculum deleted successfully.");
                }
                else
                {
                    return new ServiceResponse<bool>(false, false, "Delete operation failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting curriculum.");
                return new ServiceResponse<bool>(false, false, "Operation failed: " + ex.Message);
            }
        }
    }
}
