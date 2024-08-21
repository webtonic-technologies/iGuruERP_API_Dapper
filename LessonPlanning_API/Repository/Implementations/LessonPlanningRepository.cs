using Dapper;
using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Lesson_API.Repository.Implementations
{
    public class LessonPlanningRepository : ILessonPlanningRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<LessonPlanningRepository> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public LessonPlanningRepository(IConfiguration configuration, ILogger<LessonPlanningRepository> logger, IWebHostEnvironment hostingEnvironment)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ServiceResponse<string>> AddUpdateLessonPlanning(LessonPlanningRequest request)
        {
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.Open();
            }

            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                // Insert or Update LessonPlanning
                if (request.LessonPlanningID == 0)
                {
                    var lessonPlanningSql = @"INSERT INTO tblLessonPlanning (AcademicYear, ClassID, SectionID, SubjectID, InstituteID, IsActive) 
                                              VALUES (@AcademicYear, @ClassID, @SectionID, @SubjectID, @InstituteID, 1);
                                              SELECT CAST(SCOPE_IDENTITY() as int);";
                    request.LessonPlanningID = await _dbConnection.QuerySingleAsync<int>(lessonPlanningSql, new
                    {
                        request.AcademicYear,
                        request.ClassID,
                        request.SectionID,
                        request.SubjectID,
                        request.InstituteID
                    }, transaction);
                }
                else
                {
                    var lessonPlanningSql = @"UPDATE tblLessonPlanning SET 
                                              AcademicYear = @AcademicYear, 
                                              ClassID = @ClassID, 
                                              SectionID = @SectionID, 
                                              SubjectID = @SubjectID, 
                                              InstituteID = @InstituteID,
                                              IsActive = @IsActive
                                              WHERE LessonPlanningID = @LessonPlanningID";
                    await _dbConnection.ExecuteAsync(lessonPlanningSql, new
                    {
                        request.LessonPlanningID,
                        request.AcademicYear,
                        request.ClassID,
                        request.SectionID,
                        request.SubjectID,
                        request.InstituteID,
                        request.IsActive
                    }, transaction);
                }

                // Insert or Update LessonPlanning Information
                foreach (var info in request.LessonPlanningInformation)
                {
                    if (info.LessonPlanningInfoID == 0)
                    {
                        var infoSql = @"INSERT INTO tblLessonPlanningInformation (LessonPlanningID, LessonDate, PlanTypeID, CurriculumChapterID, CurriculumSubTopicID, Synopsis, Introduction, MainTeaching, Conclusion, Attachments) 
                                        VALUES (@LessonPlanningID, @LessonDate, @PlanTypeID, @CurriculumChapterID, @CurriculumSubTopicID, @Synopsis, @Introduction, @MainTeaching, @Conclusion, @Attachments);
                                        SELECT CAST(SCOPE_IDENTITY() as int);";
                        info.LessonPlanningInfoID = await _dbConnection.QuerySingleAsync<int>(infoSql, new
                        {
                            LessonPlanningID = request.LessonPlanningID,
                            info.LessonDate,
                            info.PlanTypeID,
                            info.CurriculumChapterID,
                            info.CurriculumSubTopicID,
                            info.Synopsis,
                            info.Introduction,
                            info.MainTeaching,
                            info.Conclusion,
                            info.Attachments
                        }, transaction);
                    }
                    else
                    {
                        var infoSql = @"UPDATE tblLessonPlanningInformation SET 
                                        LessonDate = @LessonDate, 
                                        PlanTypeID = @PlanTypeID, 
                                        CurriculumChapterID = @CurriculumChapterID, 
                                        CurriculumSubTopicID = @CurriculumSubTopicID, 
                                        Synopsis = @Synopsis, 
                                        Introduction = @Introduction, 
                                        MainTeaching = @MainTeaching, 
                                        Conclusion = @Conclusion, 
                                        Attachments = @Attachments
                                        WHERE LessonPlanningInfoID = @LessonPlanningInfoID";
                        await _dbConnection.ExecuteAsync(infoSql, new
                        {
                            info.LessonPlanningInfoID,
                            info.LessonDate,
                            info.PlanTypeID,
                            info.CurriculumChapterID,
                            info.CurriculumSubTopicID,
                            info.Synopsis,
                            info.Introduction,
                            info.MainTeaching,
                            info.Conclusion,
                            info.Attachments
                        }, transaction);
                    }
                    var docs = await AddUpdateDocuments(info.Documents, info.LessonPlanningInfoID);
                }

                transaction.Commit();
                return new ServiceResponse<string>(request.LessonPlanningID.ToString(), true, "Lesson Planning added/updated successfully.", 200, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding/updating lesson planning.");
                transaction.Rollback();
                return new ServiceResponse<string>(null, false, "Operation failed: " + ex.Message, 500, null);
            }
        }

        public async Task<ServiceResponse<List<GetAllLessonPlanningResponse>>> GetAllLessonPlanning(GetAllLessonPlanningRequest request)
        {
            try
            {
                var sql = @"
            WITH LessonPlanningData AS (
                SELECT 
                    lp.LessonPlanningID,
                    lp.AcademicYear,
                    lp.ClassID,
                    c.class_name AS ClassName,
                    sec.section_name AS SectionName,
                    lp.SubjectID,
                    s.subject_name AS SubjectName,
                    t.employee_name AS TeacherName,
                    lp.InstituteID,
                    lp.IsActive,
                    ROW_NUMBER() OVER (ORDER BY lp.LessonPlanningID) AS RowNum
                FROM tblLessonPlanning lp
                INNER JOIN tbl_Class c ON lp.ClassID = c.class_id
                INNER JOIN tbl_Section sec ON lp.SectionID = sec.section_id
                INNER JOIN tbl_InstituteSubjects s ON lp.SubjectID = s.institute_subject_id
                INNER JOIN tbl_EmployeeProfileMaster t ON lp.TeacherID = t.employee_id
                WHERE lp.InstituteID = @InstituteID AND lp.IsActive = 1
            )
            SELECT 
                lpd.LessonPlanningID,
                lpd.AcademicYear,
                lpd.ClassID,
                lpd.ClassName,
                lpd.SectionName,
                lpd.SubjectID,
                lpd.SubjectName,
                lpd.TeacherName,
                lpd.InstituteID,
                lpd.IsActive
            FROM LessonPlanningData lpd
            WHERE lpd.RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize;

            WITH LessonPlanningDataForInfo AS (
                SELECT 
                    lp.LessonPlanningID,
                    ROW_NUMBER() OVER (ORDER BY lp.LessonPlanningID) AS RowNum
                FROM tblLessonPlanning lp
                WHERE lp.InstituteID = @InstituteID AND lp.IsActive = 1
            )
            SELECT
                lpi.LessonPlanningInfoID,
                lpi.LessonPlanningID,
                lpi.LessonDate,
                lpi.PlanTypeID,
                lpi.CurriculumChapterID,
                lpi.CurriculumSubTopicID,
                lpi.Synopsis,
                lpi.Introduction,
                lpi.MainTeaching,
                lpi.Conclusion,
                lpi.Attachments
            FROM tblLessonPlanningInformation lpi
            INNER JOIN LessonPlanningDataForInfo lpd ON lpi.LessonPlanningID = lpd.LessonPlanningID
            WHERE lpd.RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize;";

                using var multi = await _dbConnection.QueryMultipleAsync(sql, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                var lessonPlannings = (await multi.ReadAsync<GetAllLessonPlanningResponse>()).ToList();
                var lessonPlanningInfos = (await multi.ReadAsync<LessonPlanningInformationResponse>()).ToList();

                foreach (var lessonPlanning in lessonPlannings)
                {
                    lessonPlanning.LessonPlanningInformation = lessonPlanningInfos
                        .Where(info => info.LessonPlanningID == lessonPlanning.LessonPlanningID)
                        .ToList();
                }

                return new ServiceResponse<List<GetAllLessonPlanningResponse>>(lessonPlannings, true, "Lesson Plannings retrieved successfully.", 200, lessonPlannings.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving lesson plannings.");
                return new ServiceResponse<List<GetAllLessonPlanningResponse>>(null, false, "Operation failed: " + ex.Message, 500, null);
            }
        }

        public async Task<ServiceResponse<LessonPlanning>> GetLessonPlanningById(int id)
        {
            try
            {
                var sql = @"
                    SELECT * FROM tblLessonPlanning WHERE LessonPlanningID = @LessonPlanningID;
                    SELECT * FROM tblLessonPlanningInformation WHERE LessonPlanningID = @LessonPlanningID;";

                using var multi = await _dbConnection.QueryMultipleAsync(sql, new { LessonPlanningID = id });

                var lessonPlanning = await multi.ReadSingleOrDefaultAsync<LessonPlanning>();
                if (lessonPlanning != null)
                {
                    lessonPlanning.LessonPlanningInformation = (await multi.ReadAsync<LessonPlanningInformation>()).ToList();
                    foreach(var data in lessonPlanning.LessonPlanningInformation)
                    {
                        data.Documents =  GetLessonPlanningDocuments(data.LessonPlanningInfoID);
                    }
                }

                return new ServiceResponse<LessonPlanning>(lessonPlanning, lessonPlanning != null, lessonPlanning != null ? "Lesson Planning found." : "Lesson Planning not found.", 200, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving lesson planning by ID.");
                return new ServiceResponse<LessonPlanning>(null, false, "Operation failed: " + ex.Message, 500, null);
            }
        }
        public async Task<ServiceResponse<int>> HardDeleteDocument(int documentId)
        {
            try
            {
                // SQL query to delete the document
                string deleteSql = @"
        DELETE FROM tblLessonDocuments 
        WHERE DocumentsId = @DocumentsId";

                // Execute the delete operation
                int affectedRows = await _dbConnection.ExecuteAsync(deleteSql, new { DocumentsId = documentId });

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
        public async Task<ServiceResponse<bool>> DeleteLessonPlanning(int id)
        {
            try
            {
                var sql = @"UPDATE tblLessonPlanning SET IsActive = 0 WHERE LessonPlanningID = @LessonPlanningID";
                var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { LessonPlanningID = id });

                return new ServiceResponse<bool>(rowsAffected > 0, rowsAffected > 0, rowsAffected > 0 ? "Lesson Planning deleted successfully." : "Delete operation failed.", 200, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting lesson planning.");
                return new ServiceResponse<bool>(false, false, "Operation failed: " + ex.Message, 500, null);
            }
        }
        private async Task<ServiceResponse<int>> AddUpdateDocuments(List<documents> request, int lessonPlanningInfoID)
        {
            if (request == null || !request.Any())
            {
                return new ServiceResponse<int>(0, false, "No documents provided to update", 400);
            }

            try
            {
                // Step 1: Hard delete existing documents for the given LessonPlanningInfoID
                string deleteSql = @"
        DELETE FROM tblLessonDocuments 
        WHERE LessonPlanningInfoID = @LessonPlanningInfoID"
                ;

                await _dbConnection.ExecuteAsync(deleteSql, new { LessonPlanningInfoID = lessonPlanningInfoID });

                // Step 2: Insert new documents
                string insertSql = @"
        INSERT INTO tblLessonDocuments (LessonPlanningInfoID, DocFile)
        VALUES (@LessonPlanningInfoID, @DocFile)";

                foreach (var doc in request)
                {
                    doc.LessonPlanningInfoID = lessonPlanningInfoID; // Ensure the LessonPlanningInfoID is set for each document
                    doc.DocFile = ImageUpload(doc.DocFile); // Assuming ImageUpload handles file uploads and returns the file path
                    await _dbConnection.ExecuteAsync(insertSql, doc);
                }

                return new ServiceResponse<int>(request.Count, true, "Documents added/updated successfully", 200);
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
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "LessonPlanning");

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
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "LessonPlanning", Filename);

            if (!File.Exists(filePath))
            {
                return string.Empty;
            }
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }
        private List<documents> GetLessonPlanningDocuments(int lessonPlanningInfoID)
        {
            try
            {
                string sql = @"
        SELECT DocumentsId, LessonPlanningInfoID, DocFile 
        FROM tblLessonDocuments 
        WHERE LessonPlanningInfoID = @LessonPlanningInfoID";

                var documentsList =  _dbConnection.Query<documents>(sql, new { LessonPlanningInfoID = lessonPlanningInfoID });
                foreach (var data in documentsList)
                {
                    data.DocFile = GetImage(data.DocFile);
                }
                if (documentsList != null && documentsList.Any())
                {
                    return documentsList.ToList();
                }
                else
                {
                    return [];
                }
            }
            catch (Exception ex)
            {
                return [];
            }
        }
    }
}
