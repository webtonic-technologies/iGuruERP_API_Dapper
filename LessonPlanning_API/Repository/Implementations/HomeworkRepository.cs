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
                    var homeworkSql = @"
                    INSERT INTO tblHomework (HomeworkName, SubjectID, HomeworkTypeID, Notes, InstituteID, IsActive, HomeWorkDate, CreatedBy) 
                    VALUES (@HomeworkName, @SubjectID, @HomeworkTypeID, @Notes, @InstituteID, 1, @HomeWorkDate, @CreatedBy);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                    request.HomeworkID = await _dbConnection.QuerySingleAsync<int>(homeworkSql, new
                    {
                        request.HomeworkName,
                        request.SubjectID,
                        request.HomeworkTypeID,
                        request.Notes,
                        request.InstituteID,
                        HomeWorkDate = DateTime.ParseExact(request.HomeWorkDate, "dd-MM-yyyy", null), // Date in 'DD-MM-YYYY' format
                        request.CreatedBy
                    }, transaction);

                    // Insert Homework Class Sections
                    foreach (var classSection in request.ClassSections)
                    {
                        var classSectionSql = @"
                        INSERT INTO tblHomeworkClassSection (HomeworkID, ClassID, SectionID) 
                        VALUES (@HomeworkID, @ClassID, @SectionID);";

                        await _dbConnection.ExecuteAsync(classSectionSql, new
                        {
                            HomeworkID = request.HomeworkID,
                            classSection.ClassID,
                            classSection.SectionID
                        }, transaction);
                    }

                    // Insert Homework Documents
                    await AddUpdateHomeworkDocs(request.HomeworkDocs, request.HomeworkID, transaction);
                }
                else
                {
                    var homeworkSql = @"
                    UPDATE tblHomework SET 
                        HomeworkName = @HomeworkName, 
                        SubjectID = @SubjectID, 
                        HomeworkTypeID = @HomeworkTypeID, 
                        Notes = @Notes, 
                        InstituteID = @InstituteID,
                        HomeWorkDate = @HomeWorkDate,
                        CreatedBy = @CreatedBy,
                        IsActive = @IsActive
                    WHERE HomeworkID = @HomeworkID;";

                    await _dbConnection.ExecuteAsync(homeworkSql, new
                    {
                        request.HomeworkID,
                        request.HomeworkName,
                        request.SubjectID,
                        request.HomeworkTypeID,
                        request.Notes,
                        request.InstituteID,
                        HomeWorkDate = DateTime.ParseExact(request.HomeWorkDate, "dd-MM-yyyy", null), // Date in 'DD-MM-YYYY' format
                        request.CreatedBy,
                        request.IsActive
                    }, transaction);

                    // Delete and re-insert Homework Class Sections
                    var deleteClassSectionsSql = @"DELETE FROM tblHomeworkClassSection WHERE HomeworkID = @HomeworkID;";
                    await _dbConnection.ExecuteAsync(deleteClassSectionsSql, new { request.HomeworkID }, transaction);

                    foreach (var classSection in request.ClassSections)
                    {
                        var classSectionSql = @"
                        INSERT INTO tblHomeworkClassSection (HomeworkID, ClassID, SectionID) 
                        VALUES (@HomeworkID, @ClassID, @SectionID);";

                        await _dbConnection.ExecuteAsync(classSectionSql, new
                        {
                            HomeworkID = request.HomeworkID,
                            classSection.ClassID,
                            classSection.SectionID
                        }, transaction);
                    }

                    // Update Homework Documents
                    await AddUpdateHomeworkDocs(request.HomeworkDocs, request.HomeworkID, transaction);
                }

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

        private async Task AddUpdateHomeworkDocs(List<HomeworkDocs> homeworkDocs, int homeworkId, IDbTransaction transaction)
        {
            // Path to the "Assets/HomeworkDocs" directory
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "HomeworkDocs");

            // Ensure the directory exists
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Delete existing documents for this homework
            string deleteSql = @"DELETE FROM tblHomeworkDocuments WHERE HomeworkID = @HomeworkID;";
            await _dbConnection.ExecuteAsync(deleteSql, new { HomeworkID = homeworkId }, transaction);

            // Insert new documents
            string insertSql = @"INSERT INTO tblHomeworkDocuments (HomeworkID, DocFile) VALUES (@HomeworkID, @DocFile);";
            foreach (var doc in homeworkDocs)
            {
                // Convert the base64 string to a byte array
                byte[] fileBytes = Convert.FromBase64String(doc.DocFile);

                // Create a unique filename (e.g., GUID with the appropriate file extension)
                string fileExtension = GetFileExtension(fileBytes);
                string fileName = $"{Guid.NewGuid()}{fileExtension}";
                string filePath = Path.Combine(directoryPath, fileName);

                // Save the file to the directory
                await File.WriteAllBytesAsync(filePath, fileBytes);

                // Insert the file path (relative or absolute) into the database
                await _dbConnection.ExecuteAsync(insertSql, new { HomeworkID = homeworkId, DocFile = fileName }, transaction);
            }
        }

        // Helper method to determine the file extension based on the byte array
        private string GetFileExtension(byte[] fileBytes)
        {
            if (IsJpeg(fileBytes)) return ".jpg";
            if (IsPng(fileBytes)) return ".png";
            if (IsGif(fileBytes)) return ".gif";
            if (IsPdf(fileBytes)) return ".pdf";
            throw new InvalidOperationException("Unsupported file format");
        }

        // Helper methods to identify the file type
        private bool IsJpeg(byte[] bytes) => bytes.Length > 2 && bytes[0] == 0xFF && bytes[1] == 0xD8;
        private bool IsPng(byte[] bytes) => bytes.Length > 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47;
        private bool IsGif(byte[] bytes) => bytes.Length > 6 && bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46;
        private bool IsPdf(byte[] bytes) => bytes.Length > 4 && bytes[0] == 0x25 && bytes[1] == 0x50 && bytes[2] == 0x44 && bytes[3] == 0x46;




        public async Task<ServiceResponse<List<GetAllHomeworkResponse>>> GetAllHomework(GetAllHomeworkRequest request)
        {
            try
            {
                // Parse the startDate and endDate to ensure 'DD-MM-YYYY' format
                DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", null);
                DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null);

                var sql = @"
        WITH HomeworkData AS (
            SELECT 
                hw.HomeworkID,
                hw.HomeworkName,
                s.SubjectName,
                ht.HomeworkType,
                hw.Notes,
                hw.IsActive,
                CONCAT(emp.First_Name, ' ', emp.Last_Name) AS CreatedBy,
                ROW_NUMBER() OVER (ORDER BY hw.HomeworkID) AS RowNum
            FROM tblHomework hw
            INNER JOIN tbl_Subjects s ON hw.SubjectID = s.SubjectId
            INNER JOIN tblHomeworkType ht ON hw.HomeworkTypeID = ht.HomeworkTypeID
            INNER JOIN tbl_EmployeeProfileMaster emp ON hw.CreatedBy = emp.Employee_id
            WHERE hw.InstituteID = @InstituteID
            AND hw.HomeWorkDate BETWEEN @StartDate AND @EndDate -- Filter by date range
            AND hw.IsActive = 1
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
            hd.CreatedBy,
            hd.IsActive,
            cs.ClassName,
            cs.SectionName
        FROM HomeworkData hd
        LEFT JOIN ClassSectionData cs ON hd.HomeworkID = cs.HomeworkID
        WHERE hd.RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize;";

                using var multi = await _dbConnection.QueryMultipleAsync(sql, new
                {
                    request.InstituteID,
                    StartDate = startDate, // Use parsed dates
                    EndDate = endDate,     // Use parsed dates
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
                    hd.IsActive,
                    hd.CreatedBy
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
                        IsActive = group.Key.IsActive,
                        CreatedBy = group.Key.CreatedBy,
                        ClassSections = group.Select(g => new ClassSectionHWResponse
                        {
                            HomeworkID = g.HomeworkID,
                            ClassName = g.ClassName,
                            SectionName = g.SectionName
                        }).ToList()
                    };

                    homeworkList.Add(homeworkResponse);
                }

                foreach (var data in homeworkList)
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

        public async Task<ServiceResponse<GetHomeworkHistoryResponse>> GetHomeworkHistory(GetHomeworkHistoryRequest request)
        {
            try
            {
                // Log the HomeworkID received in the request
                _logger.LogInformation($"Received HomeworkID: {request.HomeworkID}");

                // Get Total Students
                var totalStudentsQuery = @"
            SELECT COUNT(*) 
            FROM tbl_StudentMaster 
            WHERE class_id IN (SELECT ClassID FROM tblHomeworkClassSection WHERE HomeworkID = @HomeworkID) 
            AND section_id IN (SELECT SectionID FROM tblHomeworkClassSection WHERE HomeworkID = @HomeworkID)";

                var totalStudents = await _dbConnection.ExecuteScalarAsync<int>(totalStudentsQuery, new { request.HomeworkID });

                // Get Submitted Count
                var submittedQuery = @"
            SELECT COUNT(*) 
            FROM tblHomeWorkStatusMapping 
            WHERE HomeworkID = @HomeworkID AND SubmittedStatus = 1;";
                var submitted = await _dbConnection.ExecuteScalarAsync<int>(submittedQuery, new { request.HomeworkID });

                // Get Checked Count
                var checkedQuery = @"
            SELECT COUNT(*) 
            FROM tblHomeWorkStatusMapping 
            WHERE HomeworkID = @HomeworkID AND CheckedStatus = 1;";
                var checkedCount = await _dbConnection.ExecuteScalarAsync<int>(checkedQuery, new { request.HomeworkID });

                // Get Student Homework Status (with LEFT JOIN to ensure all students are returned)
                var studentStatusQuery = @"
            SELECT sm.student_id AS StudentID, 
                   sm.First_Name + ' ' + sm.Last_Name AS StudentName,
                   ISNULL(hsm.SeenStatus, 0) AS SeenStatus, 
                   hsm.SeenDate,
                   ISNULL(hsm.SubmittedStatus, 0) AS SubmittedStatus, 
                   hsm.SubmittedDate,
                   ISNULL(hsm.CheckedStatus, 0) AS CheckedStatus, 
                   hsm.CheckedDate
            FROM tbl_StudentMaster sm
            LEFT JOIN tblHomeWorkStatusMapping hsm ON sm.student_id = hsm.StudentID AND hsm.HomeworkID = @HomeworkID
            WHERE sm.class_id IN (SELECT ClassID FROM tblHomeworkClassSection WHERE HomeworkID = @HomeworkID)
            AND sm.section_id IN (SELECT SectionID FROM tblHomeworkClassSection WHERE HomeworkID = @HomeworkID)";

                var students = (await _dbConnection.QueryAsync<StudentHomeworkStatus>(studentStatusQuery, new { request.HomeworkID })).ToList();

                // Formatting Date and Status
                foreach (var student in students)
                {
                    // Compare integer values directly for status display
                    student.SeenStatusDisplay = student.SeenStatus == 1 ? "Seen" : "Unseen";
                    student.SubmittedStatusDisplay = student.SubmittedStatus == 1 ? "Submitted" : "Not Submitted";
                    student.CheckedStatusDisplay = student.CheckedStatus == 1 ? "Checked" : "Not Checked";

                    // Format the dates if available
                    student.SeenDateTime = student.SeenDate.HasValue
                        ? student.SeenDate.Value.ToString("dd-MM-yyyy 'at' h:mmtt").ToLower()
                        : null;

                    student.SubmittedDateTime = student.SubmittedDate.HasValue
                        ? student.SubmittedDate.Value.ToString("dd-MM-yyyy 'at' h:mmtt").ToLower()
                        : null;

                    student.CheckedDateTime = student.CheckedDate.HasValue
                        ? student.CheckedDate.Value.ToString("dd-MM-yyyy 'at' h:mmtt").ToLower()
                        : null;
                }

                // Response
                var response = new GetHomeworkHistoryResponse
                {
                    TotalStudents = totalStudents,
                    Submitted = submitted,
                    NotSubmitted = totalStudents - submitted,
                    Checked = checkedCount,
                    NotChecked = totalStudents - checkedCount,
                    Students = students
                };

                return new ServiceResponse<GetHomeworkHistoryResponse>(response, true, "Homework history retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving homework history.");
                return new ServiceResponse<GetHomeworkHistoryResponse>(null, false, $"Operation failed: {ex.Message}", 500);
            }
        }
         

    }
}
