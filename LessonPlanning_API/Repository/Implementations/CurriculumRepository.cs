using Dapper;
using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lesson_API.DTOs.Responses;

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
                    var curriculumSql = @"INSERT INTO tblCurriculum (AcademicYearID, ClassID, SubjectID, InstituteID, IsActive) 
                                  VALUES (@AcademicYearID, @ClassID, @SubjectID, @InstituteID, 1);
                                  SELECT CAST(SCOPE_IDENTITY() as int);";
                    request.CurriculumID = await _dbConnection.QuerySingleAsync<int>(curriculumSql, new
                    {
                        request.AcademicYearID,
                        request.ClassID,
                        request.SubjectID,
                        request.InstituteID
                    }, transaction);
                }
                else
                {
                    var curriculumSql = @"UPDATE tblCurriculum SET 
                                  AcademicYearID = @AcademicYearID,
                                  ClassID = @ClassID, 
                                  SubjectID = @SubjectID, 
                                  InstituteID = @InstituteID
                                  WHERE CurriculumID = @CurriculumID";
                    await _dbConnection.ExecuteAsync(curriculumSql, new
                    {
                        request.CurriculumID,
                        request.AcademicYearID,
                        request.ClassID,
                        request.SubjectID,
                        request.InstituteID
                    }, transaction);
                }

                // Handle chapters and subtopics (with file saving logic)
                await HandleChaptersAndSubTopics(request, transaction);

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

        private async Task HandleChaptersAndSubTopics(CurriculumRequest request, IDbTransaction transaction)
        {
            foreach (var chapter in request.CurriculumChapters)
            {
                // Insert or Update Chapter
                if (chapter.CurriculumChapterID == 0)
                {
                    var chapterSql = @"INSERT INTO tblCurriculumChapter (ChapterName, TotalSessions, CurriculumID, InstituteID, IsActive) 
                               VALUES (@ChapterName, @TotalSessions, @CurriculumID, @InstituteID, 1);
                               SELECT CAST(SCOPE_IDENTITY() as int);";
                    chapter.CurriculumChapterID = await _dbConnection.QuerySingleAsync<int>(chapterSql, new
                    {
                        chapter.ChapterName,
                        chapter.TotalSessions,
                        CurriculumID = request.CurriculumID,
                        request.InstituteID
                    }, transaction);
                }
                else
                {
                    var chapterSql = @"UPDATE tblCurriculumChapter SET 
                               ChapterName = @ChapterName, 
                               TotalSessions = @TotalSessions
                               WHERE CurriculumChapterID = @CurriculumChapterID";
                    await _dbConnection.ExecuteAsync(chapterSql, new
                    {
                        chapter.ChapterName,
                        chapter.TotalSessions,
                        chapter.CurriculumChapterID
                    }, transaction);
                }

                // Handle Chapter Docs
                await AddUpdateChapterDocs(chapter.chapterDocs, chapter.CurriculumChapterID, transaction);

                // Insert or Update SubTopics and their Documents
                foreach (var subTopic in chapter.CurriculumSubTopics)
                {
                    if (subTopic.CurriculumSubTopicID == 0)
                    {
                        var subTopicSql = @"INSERT INTO tblCurriculumSubTopic (SubTopicName, TotalSession, CurriculumChapterID, InstituteID, IsActive) 
                                    VALUES (@SubTopicName, @TotalSession, @CurriculumChapterID, @InstituteID, 1);
                                    SELECT CAST(SCOPE_IDENTITY() as int);";
                        subTopic.CurriculumSubTopicID = await _dbConnection.QuerySingleAsync<int>(subTopicSql, new
                        {
                            subTopic.SubTopicName,
                            subTopic.TotalSession,
                            CurriculumChapterID = chapter.CurriculumChapterID,
                            request.InstituteID
                        }, transaction);
                    }
                    else
                    {
                        var subTopicSql = @"UPDATE tblCurriculumSubTopic SET 
                                    SubTopicName = @SubTopicName, 
                                    TotalSession = @TotalSession
                                    WHERE CurriculumSubTopicID = @CurriculumSubTopicID";
                        await _dbConnection.ExecuteAsync(subTopicSql, new
                        {
                            subTopic.SubTopicName,
                            subTopic.TotalSession,
                            subTopic.CurriculumSubTopicID
                        }, transaction);
                    }

                    // Handle SubTopic Docs
                    await AddUpdateSubtopicDocs(subTopic.SubtopicDocs, subTopic.CurriculumSubTopicID, transaction);

                    // Insert or Update Curriculum Resource Details
                    if (subTopic.CurriculumResourceDetails != null && subTopic.CurriculumResourceDetails.Count > 0)
                    {
                        await AddUpdateCurriculumResourceDetails(subTopic.CurriculumResourceDetails, subTopic.CurriculumSubTopicID, transaction);
                    }
                }
            }
        }

        private async Task AddUpdateCurriculumResourceDetails(List<CurriculumResourceDetailsRequest> resourceDetails, int subTopicId, IDbTransaction transaction)
        {
            foreach (var resourceDetail in resourceDetails)
            {
                if (resourceDetail.CurriculumResourceID == 0)
                {
                    // Insert new resource detail
                    var resourceDetailSql = @"INSERT INTO tblCurriculumResourceDetails 
                                       (CurriculumSubTopicID, LearningObjectives, SuggestedActivity, TeachingResouces, TeachingMethod, Criteria, IsActive)
                                       VALUES (@CurriculumSubTopicID, @LearningObjectives, @SuggestedActivity, @TeachingResouces, @TeachingMethod, @Criteria, 1);
                                       SELECT CAST(SCOPE_IDENTITY() as int);";

                    await _dbConnection.ExecuteAsync(resourceDetailSql, new
                    {
                        CurriculumSubTopicID = subTopicId,
                        resourceDetail.LearningObjectives,
                        resourceDetail.SuggestedActivity,
                        resourceDetail.TeachingResouces,
                        resourceDetail.TeachingMethod,
                        resourceDetail.Criteria
                    }, transaction);
                }
                else
                {
                    // Update existing resource detail
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


        private async Task AddUpdateChapterDocs(List<chapterDocs> chapterDocs, int chapterId, IDbTransaction transaction)
        {
            foreach (var doc in chapterDocs)
            {
                doc.DocFile = SaveBase64File(doc.DocFile, "ChapterDocs");
                var docSql = @"INSERT INTO tblChapterDocuments (CurriculumChapterID, DocFile) 
                       VALUES (@CurriculumChapterID, @DocFile)";
                await _dbConnection.ExecuteAsync(docSql, new { CurriculumChapterID = chapterId, DocFile = doc.DocFile }, transaction);
            }
        }

        private async Task AddUpdateSubtopicDocs(List<SubtopicDocs> subtopicDocs, int subtopicId, IDbTransaction transaction)
        {
            foreach (var doc in subtopicDocs)
            {
                doc.DocFile = SaveBase64File(doc.DocFile, "SubtopicDocs");
                var docSql = @"INSERT INTO tblSubtopicDocuments (CurriculumSubTopicID, DocFile) 
                       VALUES (@CurriculumSubTopicID, @DocFile)";
                await _dbConnection.ExecuteAsync(docSql, new { CurriculumSubTopicID = subtopicId, DocFile = doc.DocFile }, transaction);
            }
        }

        private string SaveBase64File(string base64File, string folderName)
        {
            if (string.IsNullOrEmpty(base64File))
                throw new ArgumentException("Base64 file content cannot be null or empty.");

            if (string.IsNullOrEmpty(_hostingEnvironment.WebRootPath))
                throw new InvalidOperationException("WebRootPath is not configured properly.");  // This error is currently being thrown

            byte[] fileBytes = Convert.FromBase64String(base64File);

            // Ensure WebRootPath is valid and folder exists
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Assets", folderName);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);  // Ensure directory exists
            }

            string fileName = $"{Guid.NewGuid()}.pdf";  // Or other extension based on file type
            string fullPath = Path.Combine(filePath, fileName);

            // Save the file to disk
            File.WriteAllBytes(fullPath, fileBytes);

            return fileName;  // Return the file name that will be saved in the database
        }

        public async Task<ServiceResponse<List<GetAllCurriculumResponse>>> GetAllCurriculum(GetAllCurriculumRequest request)
        {
            try
            {
                var sql = @"
            WITH CurriculumData AS (
                SELECT 
                    s.SubjectID, 
                    s.SubjectName, 
                    c.class_name as ClassName, 
                    sec.section_name as SectionName,
                    COUNT(ch.CurriculumChapterID) AS NoOfChapters,
                    ROW_NUMBER() OVER (ORDER BY s.SubjectName) AS RowNum
                FROM tblCurriculum cu
                INNER JOIN tbl_Subjects s ON cu.SubjectID = s.SubjectId
                INNER JOIN tbl_Class c ON cu.ClassID = c.class_id
                INNER JOIN tbl_Section sec ON sec.class_id = c.class_id
                LEFT JOIN tblCurriculumChapter ch ON cu.CurriculumID = ch.CurriculumID
                WHERE cu.InstituteID = @InstituteID 
                AND cu.ClassID = @ClassID 
                AND cu.AcademicYearID = @AcademicYearID
                AND cu.IsActive = 1
                GROUP BY s.SubjectID, s.SubjectName, c.class_name, sec.section_name
            )
            SELECT SubjectID, SubjectName, ClassName, SectionName, NoOfChapters
            FROM CurriculumData";

                var result = await _dbConnection.QueryAsync<dynamic>(sql, new
                {
                    request.AcademicYearID,
                    request.ClassID,
                    request.InstituteID
                });

                // Grouping data based on SubjectId and SubjectName to match the response format
                var groupedResult = result
                    .GroupBy(r => new { SubjectId = r.SubjectID, SubjectName = r.SubjectName })
                    .Select(g => new GetAllCurriculumResponse
                    {
                        SubjectId = g.Key.SubjectId,
                        SubjectName = g.Key.SubjectName,
                        ClassSection = g.Select(c => new ClassSection
                        {
                            ClassName = c.ClassName,
                            SectionName = c.SectionName
                        }).ToList(),
                        NoOfChapters = g.Sum(c => c.NoOfChapters)
                    }).ToList();

                return new ServiceResponse<List<GetAllCurriculumResponse>>(groupedResult, true, "Curriculums retrieved successfully.", 200);
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
                if (curriculum != null)
                {
                    curriculum.CurriculumChapters = await GetCurriculumChapters(curriculum.CurriculumID);
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

        private async Task<List<CurriculumChapter1>> GetCurriculumChapters(int curriculumID)
        {
            // Logic to get Curriculum Chapters
            return new List<CurriculumChapter1>();
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

        private string ImageUpload(string base64Image)
        {
            if (string.IsNullOrEmpty(base64Image))
            {
                throw new ArgumentException("Base64 image string cannot be null or empty.");
            }

            byte[] imageData = Convert.FromBase64String(base64Image);
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "CurriculumDocs");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string fileExtension = GetFileExtension(imageData);
            if (string.IsNullOrEmpty(fileExtension))
            {
                throw new InvalidOperationException("Unsupported file format.");
            }

            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(directoryPath, fileName);

            File.WriteAllBytes(filePath, imageData);
            return filePath;
        }

        private string GetFileExtension(byte[] fileData)
        {
            if (IsJpeg(fileData)) return ".jpg";
            if (IsPng(fileData)) return ".png";
            if (IsGif(fileData)) return ".gif";
            if (IsPdf(fileData)) return ".pdf";
            return null;
        }

        // Method to check if the byte array is a JPEG
        private bool IsJpeg(byte[] bytes)
        {
            // JPEG files start with FF D8 and end with FF D9
            return bytes.Length > 1 && bytes[0] == 0xFF && bytes[1] == 0xD8;
        }

        // Method to check if the byte array is a PNG
        private bool IsPng(byte[] bytes)
        {
            // PNG files start with 89 50 4E 47 0D 0A 1A 0A
            return bytes.Length > 7 &&
                   bytes[0] == 0x89 && bytes[1] == 0x50 &&
                   bytes[2] == 0x4E && bytes[3] == 0x47 &&
                   bytes[4] == 0x0D && bytes[5] == 0x0A &&
                   bytes[6] == 0x1A && bytes[7] == 0x0A;
        }

        // Method to check if the byte array is a GIF
        private bool IsGif(byte[] bytes)
        {
            // GIF files start with GIF87a or GIF89a
            return bytes.Length > 5 &&
                   bytes[0] == 0x47 && bytes[1] == 0x49 &&
                   bytes[2] == 0x46 && (bytes[3] == 0x38 && (bytes[4] == 0x37 || bytes[4] == 0x39)) && bytes[5] == 0x61;
        }

        // Method to check if the byte array is a PDF
        private bool IsPdf(byte[] bytes)
        {
            // PDF files start with 25 50 44 46
            return bytes.Length > 3 &&
                   bytes[0] == 0x25 && bytes[1] == 0x50 &&
                   bytes[2] == 0x44 && bytes[3] == 0x46;
        }


    }
}
