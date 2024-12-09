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
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Lesson_API.Repository.Implementations
{
    public class CurriculumRepository : ICurriculumRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<CurriculumRepository> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public CurriculumRepository(IConfiguration configuration, ILogger<CurriculumRepository> logger, IWebHostEnvironment hostingEnvironment)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
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
                        var docs = await AddUpdateChapterDocs(chapter.chapterDocs, chapter.CurriculumChapterID);
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
                        var docs = await AddUpdateSubtopicDocs(subTopic.SubtopicDocs, subTopic.CurriculumSubTopicID);
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
                return new ServiceResponse<string>(request.CurriculumID.ToString(), true, "Curriculum added/updated successfully.", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding/updating curriculum.");
                transaction.Rollback();
                return new ServiceResponse<string>(null, false, "Operation failed: " + ex.Message, 500);
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

                return new ServiceResponse<List<GetAllCurriculumResponse>>(result.ToList(), true, "Curriculums retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving curriculums.");
                return new ServiceResponse<List<GetAllCurriculumResponse>>(null, false, "Operation failed: " + ex.Message, 500);
            }
        }
        public async Task<ServiceResponse<Curriculum>> GetCurriculumById(int id)
        {
            try
            {
                var sql = @"SELECT * FROM tblCurriculum WHERE CurriculumID = @CurriculumID AND IsActive = 1";
                var curriculum = await _dbConnection.QueryFirstOrDefaultAsync<Curriculum>(sql, new { CurriculumID = id });
                foreach (var data in curriculum.CurriculumChapters)
                {
                    data.chapterDocs = GetChapterDocs(data.CurriculumChapterID);
                    foreach (var item in data.CurriculumSubTopics)
                    {
                        item.SubtopicDocs = GetSubtopicDocs(item.CurriculumSubTopicID);
                    }
                }
                if (curriculum != null)
                {
                    return new ServiceResponse<Curriculum>(curriculum, true, "Curriculum found.", 200);
                }
                else
                {
                    return new ServiceResponse<Curriculum>(null, false, "Curriculum not found.", 404);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving curriculum by ID.");
                return new ServiceResponse<Curriculum>(null, false, "Operation failed: " + ex.Message, 500);
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
                    return new ServiceResponse<bool>(true, true, "Curriculum deleted successfully.", 200);
                }
                else
                {
                    return new ServiceResponse<bool>(false, false, "Delete operation failed.", 404);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting curriculum.");
                return new ServiceResponse<bool>(false, false, "Operation failed: " + ex.Message, 500);
            }
        }
        public async Task<ServiceResponse<int>> HardDeleteChapterDoc(int documentId)
        {
            try
            {
                string deleteSql = @"
                                    DELETE FROM tblChapterDocuments 
                                    WHERE DocumentsId = @DocumentsId";

                var affectedRows = await _dbConnection.ExecuteAsync(deleteSql, new { DocumentsId = documentId });

                if (affectedRows > 0)
                {
                    return new ServiceResponse<int>(affectedRows, true, "Document deleted successfully", 200);
                }
                else
                {
                    return new ServiceResponse<int>(0, false, "Document not found", 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(0, false, ex.Message, 500);
            }
        }
        public async Task<ServiceResponse<int>> HardDeleteSubtopicDoc(int documentId)
        {
            try
            {
                string deleteSql = @"
                                    DELETE FROM tblSubtopicDocuments 
                                    WHERE DocumentsId = @DocumentsId";

                var affectedRows = await _dbConnection.ExecuteAsync(deleteSql, new { DocumentsId = documentId });

                if (affectedRows > 0)
                {
                    return new ServiceResponse<int>(affectedRows, true, "Document deleted successfully", 200);
                }
                else
                {
                    return new ServiceResponse<int>(0, false, "Document not found", 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(0, false, ex.Message, 500);
            }
        }
        private async Task<ServiceResponse<int>> AddUpdateChapterDocs(List<chapterDocs> request, int chapterId)
        {
            if (request == null || !request.Any())
            {
                return new ServiceResponse<int>(0, false, "No documents provided to update", 500);
            }

            try
            {
                // Step 1: Hard delete existing documents for the given Chapter ID
                string deleteSql = @"
                                    DELETE FROM tblChapterDocuments 
                                    WHERE CurriculumChapterID = @CurriculumChapterID"
                ;

                await _dbConnection.ExecuteAsync(deleteSql, new { CurriculumChapterID = chapterId });

                // Step 2: Insert new documents
                string insertSql = @"
                                    INSERT INTO tblChapterDocuments (CurriculumChapterID, DocFile)
                                    VALUES (@CurriculumChapterID, @DocFile)";

                foreach (var doc in request)
                {
                    doc.CurriculumChapterID = chapterId; // Ensure the Chapter ID is set for each document
                    doc.DocFile = ImageUpload(doc.DocFile); // Assuming ImageUpload handles the file upload
                    await _dbConnection.ExecuteAsync(insertSql, doc);
                }

                return new ServiceResponse<int>(request.Count,true, "Documents added/updated successfully", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(0, false, ex.Message, 500);
            }
        }
        private string ImageUpload(string image)
        {
            if (string.IsNullOrEmpty(image) || image == "string")
            {
                return string.Empty;
            }
            byte[] imageData = Convert.FromBase64String(image);
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "CurriculumDocs");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fileExtension = IsJpeg(imageData) == true ? ".jpg" : IsPng(imageData) == true ? ".png" : IsGif(imageData) == true ? ".gif" : IsPdf(imageData) == true ? ".pdf" : string.Empty;
            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(directoryPath, fileName);
            if (string.IsNullOrEmpty(fileExtension))
            {
                throw new InvalidOperationException("Incorrect file uploaded");
            }
            // Write the byte array to the image file
            File.WriteAllBytes(filePath, imageData);
            return filePath;
        }
        private bool IsJpeg(byte[] bytes)
        {
            // JPEG magic number: 0xFF, 0xD8
            return bytes.Length > 1 && bytes[0] == 0xFF && bytes[1] == 0xD8;
        }
        private bool IsPng(byte[] bytes)
        {
            // PNG magic number: 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
            return bytes.Length > 7 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47
                && bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A;
        }
        private bool IsGif(byte[] bytes)
        {
            // GIF magic number: "GIF"
            return bytes.Length > 2 && bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46;
        }
        private bool IsPdf(byte[] fileData)
        {
            return fileData.Length > 4 &&
                   fileData[0] == 0x25 && fileData[1] == 0x50 && fileData[2] == 0x44 && fileData[3] == 0x46;
        }
        private string GetImage(string Filename)
        {
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "CurriculumDocs", Filename);

            if (!File.Exists(filePath))
            {
                return string.Empty;
            }
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }
        private async Task<ServiceResponse<int>> AddUpdateSubtopicDocs(List<SubtopicDocs> request, int subtopicId)
        {
            if (request == null || !request.Any())
            {
                return new ServiceResponse<int>(0, false, "No documents provided to update", 500);
            }

            try
            {
                // Step 1: Hard delete existing documents for the given Subtopic ID
                string deleteSql = @"
                                    DELETE FROM tblSubtopicDocuments 
                                    WHERE CurriculumSubTopicID = @CurriculumSubTopicID";

                await _dbConnection.ExecuteAsync(deleteSql, new { CurriculumSubTopicID = subtopicId });

                // Step 2: Insert new documents
                string insertSql = @"
                                    INSERT INTO tblSubtopicDocuments (CurriculumSubTopicID, DocFile)
                                    VALUES (@CurriculumSubTopicID, @DocFile)";

                foreach (var doc in request)
                {
                    doc.CurriculumSubTopicID = subtopicId; // Ensure the Subtopic ID is set for each document
                    doc.DocFile = ImageUpload(doc.DocFile); // Assuming ImageUpload handles the file upload
                    await _dbConnection.ExecuteAsync(insertSql, doc);
                }

                return new ServiceResponse<int>(request.Count, true, "Documents added/updated successfully", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(0, false, ex.Message, 500);
            }
        }
        private List<chapterDocs> GetChapterDocs(int chapterId)
        {
            try
            {
                string sql = @"
                            SELECT DocumentsId, CurriculumChapterID, DocFile 
                            FROM tblChapterDocuments 
                            WHERE CurriculumChapterID = @CurriculumChapterID";

                var documents =  _dbConnection.Query<chapterDocs>(sql, new { CurriculumChapterID = chapterId });
                foreach(var data in documents)
                {
                    data.DocFile = GetImage(data.DocFile);
                }
                return documents.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private List<SubtopicDocs> GetSubtopicDocs(int subtopicId)
        {
            try
            {
                string sql = @"
                            SELECT DocumentsId, CurriculumSubTopicID, DocFile 
                            FROM tblSubtopicDocuments 
                            WHERE CurriculumSubTopicID = @CurriculumSubTopicID";

                var documents =  _dbConnection.Query<SubtopicDocs>(sql, new { CurriculumSubTopicID = subtopicId });
                foreach (var data in documents)
                {
                    data.DocFile = GetImage(data.DocFile);
                }
                return documents.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
