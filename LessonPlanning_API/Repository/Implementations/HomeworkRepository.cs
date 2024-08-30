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
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Lesson_API.Repository.Implementations
{
    public class HomeworkRepository : IHomeworkRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<HomeworkRepository> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public HomeworkRepository(IConfiguration configuration, ILogger<HomeworkRepository> logger, IWebHostEnvironment hostingEnvironment)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ServiceResponse<string>> AddUpdateHomework(HomeworkRequest request)
        {
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.Open();
            }

            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                // Insert or Update Homework
                if (request.HomeworkID == 0)
                {
                    var homeworkSql = @"INSERT INTO tblHomework (HomeworkName, SubjectID, HomeworkTypeID, Notes, Attachments, InstituteID, IsActive) 
                                        VALUES (@HomeworkName, @SubjectID, @HomeworkTypeID, @Notes, @Attachments, @InstituteID, 1);
                                        SELECT CAST(SCOPE_IDENTITY() as int);";
                    request.HomeworkID = await _dbConnection.QuerySingleAsync<int>(homeworkSql, new
                    {
                        request.HomeworkName,
                        request.SubjectID,
                        request.HomeworkTypeID,
                        request.Notes,
                        request.Attachments,
                        request.InstituteID
                    }, transaction);

                    foreach (var classSection in request.ClassSections)
                    {
                        var classSectionSql = @"INSERT INTO tblHomeworkClassSection (HomeworkID, ClassID, SectionID) 
                                                VALUES (@HomeworkID, @ClassID, @SectionID);";
                        await _dbConnection.ExecuteAsync(classSectionSql, new
                        {
                            HomeworkID = request.HomeworkID,
                            classSection.ClassID,
                            classSection.SectionID
                        }, transaction);
                    }
                }
                else
                {
                    var homeworkSql = @"UPDATE tblHomework SET 
                                        HomeworkName = @HomeworkName, 
                                        SubjectID = @SubjectID, 
                                        HomeworkTypeID = @HomeworkTypeID, 
                                        Notes = @Notes, 
                                        Attachments = @Attachments,
                                        InstituteID = @InstituteID,
                                        IsActive = @IsActive
                                        WHERE HomeworkID = @HomeworkID";
                    await _dbConnection.ExecuteAsync(homeworkSql, new
                    {
                        request.HomeworkID,
                        request.HomeworkName,
                        request.SubjectID,
                        request.HomeworkTypeID,
                        request.Notes,
                        request.Attachments,
                        request.InstituteID,
                        request.IsActive
                    }, transaction);

                    var deleteClassSectionsSql = @"DELETE FROM tblHomeworkClassSection WHERE HomeworkID = @HomeworkID";
                    await _dbConnection.ExecuteAsync(deleteClassSectionsSql, new { request.HomeworkID }, transaction);

                    foreach (var classSection in request.ClassSections)
                    {
                        var classSectionSql = @"INSERT INTO tblHomeworkClassSection (HomeworkID, ClassID, SectionID) 
                                                VALUES (@HomeworkID, @ClassID, @SectionID);";
                        await _dbConnection.ExecuteAsync(classSectionSql, new
                        {
                            HomeworkID = request.HomeworkID,
                            classSection.ClassID,
                            classSection.SectionID
                        }, transaction);
                    }
                }
                var docs = await AddUpdateHomeworkDocs(request.HomeworkDocs, request.HomeworkID);
                transaction.Commit();
                return new ServiceResponse<string>(request.HomeworkID.ToString(), true, "Homework added/updated successfully.", 200, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding/updating homework.");
                transaction.Rollback();
                return new ServiceResponse<string>(null, false, "Operation failed: " + ex.Message, 500, null);
            }
        }
        public async Task<ServiceResponse<List<GetAllHomeworkResponse>>> GetAllHomework(GetAllHomeworkRequest request)
        {
            try
            {
                var sql = @"
            WITH HomeworkData AS (
                SELECT 
                    hw.HomeworkID,
                    hw.HomeworkName,
                    s.subject_name AS SubjectName,
                    ht.HomeworkType,
                    hw.Notes,
                    hw.Attachments,
                    hw.IsActive,
                    ROW_NUMBER() OVER (ORDER BY hw.HomeworkID) AS RowNum
                FROM tblHomework hw
                INNER JOIN tbl_InstituteSubjects s ON hw.SubjectID = s.institute_subject_id
                INNER JOIN tblHomeworkType ht ON hw.HomeworkTypeID = ht.HomeworkTypeID
                WHERE hw.InstituteID = @InstituteID AND hw.IsActive = 1
            ), ClassSectionData AS (
                SELECT 
                    hcs.HomeworkID,
                    c.class_name AS ClassName,
                    sec.section_name AS SectionName
                FROM tblHomeworkClassSection hcs
                INNER JOIN tbl_Class c ON hcs.ClassID = c.class_id
                INNER JOIN tbl_Section sec ON hcs.SectionID = sec.section_id
                WHERE hcs.HomeworkID IN (SELECT HomeworkID FROM HomeworkData WHERE RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize)
            )
            SELECT 
                hd.HomeworkID,
                hd.HomeworkName,
                hd.SubjectName,
                hd.HomeworkType,
                hd.Notes,
                hd.Attachments,
                hd.IsActive,
                cs.ClassName,
                cs.SectionName
            FROM HomeworkData hd
            LEFT JOIN ClassSectionData cs ON hd.HomeworkID = cs.HomeworkID
            WHERE hd.RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize;";

                using var multi = await _dbConnection.QueryMultipleAsync(sql, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                var homeworkList = new List<GetAllHomeworkResponse>();

                var homeworkData = await multi.ReadAsync<dynamic>();
                var groupedData = homeworkData.GroupBy(hd => new
                {
                    hd.HomeworkID,
                    hd.HomeworkName,
                    hd.SubjectName,
                    hd.HomeworkType,
                    hd.Notes,
                    hd.Attachments,
                    hd.IsActive
                });

                foreach (var group in groupedData)
                {
                    var homeworkResponse = new GetAllHomeworkResponse
                    {
                        HomeworkID = group.Key.HomeworkID,
                        HomeworkName = group.Key.HomeworkName,
                        SubjectName = group.Key.SubjectName,
                        HomeworkType = group.Key.HomeworkType,
                        Notes = group.Key.Notes,
                        Attachments = group.Key.Attachments,
                        IsActive = group.Key.IsActive,
                        ClassSections = group.Select(g => new ClassSectionResponse
                        {
                            HomeworkID = g.HomeworkID,
                            ClassName = g.ClassName,
                            SectionName = g.SectionName
                        }).ToList()
                    };

                    homeworkList.Add(homeworkResponse);
                }
                foreach(var data in homeworkList)
                {
                    data.HomeworkDocs = await GetHomeworkDocuments(data.HomeworkID);
                }
                return new ServiceResponse<List<GetAllHomeworkResponse>>(homeworkList, true, "Homeworks retrieved successfully.", 200, homeworkList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving homeworks.");
                return new ServiceResponse<List<GetAllHomeworkResponse>>(null, false, "Operation failed: " + ex.Message, 500, null);
            }
        }
        public async Task<ServiceResponse<Homework>> GetHomeworkById(int id)
        {
            try
            {
                var sql = @"
            SELECT 
                hw.HomeworkID,
                hw.HomeworkName,
                hw.SubjectID,
                s.subject_name AS SubjectName,
                hw.HomeworkTypeID,
                ht.HomeworkType,
                hw.Notes,
                hw.Attachments,
                hw.InstituteID,
                hw.IsActive
            FROM tblHomework hw
            INNER JOIN tbl_InstituteSubjects s ON hw.SubjectID = s.institute_subject_id
            INNER JOIN tblHomeworkType ht ON hw.HomeworkTypeID = ht.HomeworkTypeID
            WHERE hw.HomeworkID = @HomeworkID;

            SELECT 
                hcs.HomeworkID,
                hcs.ClassID,
                c.class_name AS ClassName,
                hcs.SectionID,
                sec.section_name AS SectionName
            FROM tblHomeworkClassSection hcs
            INNER JOIN tbl_Class c ON hcs.ClassID = c.class_id
            INNER JOIN tbl_Section sec ON hcs.SectionID = sec.section_id
            WHERE hcs.HomeworkID = @HomeworkID;";

                using var multi = await _dbConnection.QueryMultipleAsync(sql, new { HomeworkID = id });

                var homework = await multi.ReadSingleOrDefaultAsync<Homework>();
                if (homework != null)
                {
                    homework.ClassSections = (await multi.ReadAsync<HomeworkClassSection>()).ToList();
                    homework.HomeworkDocs = await GetHomeworkDocuments(id);
                }

                return new ServiceResponse<Homework>(homework, homework != null, homework != null ? "Homework found." : "Homework not found.", 200, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving homework by ID.");
                return new ServiceResponse<Homework>(null, false, "Operation failed: " + ex.Message, 500, null);
            }
        }
        public async Task<ServiceResponse<bool>> DeleteHomework(int id)
        {
            try
            {
                var sql = @"UPDATE tblHomework SET IsActive = 0 WHERE HomeworkID = @HomeworkID";
                var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { HomeworkID = id });

                return new ServiceResponse<bool>(rowsAffected > 0, rowsAffected > 0, rowsAffected > 0 ? "Homework deleted successfully." : "Delete operation failed.", 200, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting homework.");
                return new ServiceResponse<bool>(false, false, "Operation failed: " + ex.Message, 500, null);
            }
        }
        private async Task<ServiceResponse<int>> AddUpdateHomeworkDocs(List<HomeworkDocs> request, int homeworkId)
        {
            if (request == null || !request.Any())
            {
                return new ServiceResponse<int>(0, false, "No documents provided to update", 400);
            }

            try
            {
                // Step 1: Hard delete existing documents for the given HomeworkID
                string deleteSql = @"
        DELETE FROM tblHomeworkDocuments 
        WHERE HomeworkID = @HomeworkID"
                ;

                await _dbConnection.ExecuteAsync(deleteSql, new { HomeworkID = homeworkId });

                // Step 2: Insert new documents
                string insertSql = @"
        INSERT INTO tblHomeworkDocuments (HomeworkID, DocFile)
        VALUES (@HomeworkID, @DocFile)";

                foreach (var doc in request)
                {
                    doc.HomeworkID = homeworkId; // Ensure the HomeworkID is set for each document
                    doc.DocFile = ImageUpload(doc.DocFile); // Assuming ImageUpload handles file uploads
                    await _dbConnection.ExecuteAsync(insertSql, doc);
                }

                return new ServiceResponse<int>(request.Count, true, "Documents added/updated successfully", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(0, false, ex.Message, 500);
            }
        }
        private async Task<List<HomeworkDocs>> GetHomeworkDocuments(int homeworkId)
        {
            try
            {
                // SQL query to select homework documents for the given HomeworkID
                string sql = @"
        SELECT DocumentsId, HomeworkID, DocFile
        FROM tblHomeworkDocuments
        WHERE HomeworkID = @HomeworkID";

                // Fetching the documents from the database
                var documents = await _dbConnection.QueryAsync<HomeworkDocs>(sql, new { HomeworkID = homeworkId });

                return documents.ToList(); // Convert to list and return
            }
            catch (Exception ex)
            {
                // Handle the exception as needed (logging, rethrowing, etc.)
                throw new Exception("Error retrieving homework documents: " + ex.Message);
            }
        }
        private string ImageUpload(string image)
        {
            if (string.IsNullOrEmpty(image) || image == "string")
            {
                return string.Empty;
            }
            byte[] imageData = Convert.FromBase64String(image);
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "HomeworkDocs");

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
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "HomeworkDocs", Filename);

            if (!File.Exists(filePath))
            {
                return string.Empty;
            }
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }
    }
}
