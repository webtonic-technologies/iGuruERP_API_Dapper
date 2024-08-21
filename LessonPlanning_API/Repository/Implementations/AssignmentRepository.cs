using Dapper;
using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Lesson_API.Repository.Implementations
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<AssignmentRepository> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public AssignmentRepository(IConfiguration configuration, ILogger<AssignmentRepository> logger, IWebHostEnvironment hostingEnvironment)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ServiceResponse<string>> AddUpdateAssignment(AssignmentRequest request)
        {
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.Open();
            }

            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                // Insert or Update Assignment
                if (request.AssignmentID == 0)
                {
                    var assignmentSql = @"INSERT INTO tblAssignment (AssignmentName, IsClasswise, IsStudentwise, ClassID, SectionID, StudentID, SubjectID, AssignmentTypeID, StartDate, SubmissionDate, Description, Reference, Attachments, InstituteID, IsActive) 
                                        VALUES (@AssignmentName, @IsClasswise, @IsStudentwise, @ClassID, @SectionID, @StudentID, @SubjectID, @AssignmentTypeID, @StartDate, @SubmissionDate, @Description, @Reference, @Attachments, @InstituteID, 1);
                                        SELECT CAST(SCOPE_IDENTITY() as int);";
                    request.AssignmentID = await _dbConnection.QuerySingleAsync<int>(assignmentSql, new
                    {
                        request.AssignmentName,
                        request.IsClasswise,
                        request.IsStudentwise,
                        request.ClassID,
                        request.SectionID,
                        request.StudentID,
                        request.SubjectID,
                        request.AssignmentTypeID,
                        request.StartDate,
                        request.SubmissionDate,
                        request.Description,
                        request.Reference,
                        request.Attachments,
                        request.InstituteID
                    }, transaction);

                    foreach (var classSection in request.ClassSections)
                    {
                        var classSectionSql = @"INSERT INTO tblAssignmentClassSection (AssignmentID, ClassID, SectionID) 
                                                VALUES (@AssignmentID, @ClassID, @SectionID);";
                        await _dbConnection.ExecuteAsync(classSectionSql, new
                        {
                            AssignmentID = request.AssignmentID,
                            classSection.ClassID,
                            classSection.SectionID
                        }, transaction);
                    }
                }
                else
                {
                    var assignmentSql = @"UPDATE tblAssignment SET 
                                        AssignmentName = @AssignmentName, 
                                        IsClasswise = @IsClasswise,
                                        IsStudentwise = @IsStudentwise,
                                        ClassID = @ClassID, 
                                        SectionID = @SectionID,
                                        StudentID = @StudentID,
                                        SubjectID = @SubjectID, 
                                        AssignmentTypeID = @AssignmentTypeID,
                                        StartDate = @StartDate,
                                        SubmissionDate = @SubmissionDate,
                                        Description = @Description,
                                        Reference = @Reference,
                                        Attachments = @Attachments,
                                        InstituteID = @InstituteID,
                                        IsActive = @IsActive
                                        WHERE AssignmentID = @AssignmentID";
                    await _dbConnection.ExecuteAsync(assignmentSql, new
                    {
                        request.AssignmentID,
                        request.AssignmentName,
                        request.IsClasswise,
                        request.IsStudentwise,
                        request.ClassID,
                        request.SectionID,
                        request.StudentID,
                        request.SubjectID,
                        request.AssignmentTypeID,
                        request.StartDate,
                        request.SubmissionDate,
                        request.Description,
                        request.Reference,
                        request.Attachments,
                        request.InstituteID,
                        request.IsActive
                    }, transaction);

                    var deleteClassSectionsSql = @"DELETE FROM tblAssignmentClassSection WHERE AssignmentID = @AssignmentID";
                    await _dbConnection.ExecuteAsync(deleteClassSectionsSql, new { request.AssignmentID }, transaction);

                    foreach (var classSection in request.ClassSections)
                    {
                        var classSectionSql = @"INSERT INTO tblAssignmentClassSection (AssignmentID, ClassID, SectionID) 
                                                VALUES (@AssignmentID, @ClassID, @SectionID);";
                        await _dbConnection.ExecuteAsync(classSectionSql, new
                        {
                            AssignmentID = request.AssignmentID,
                            classSection.ClassID,
                            classSection.SectionID
                        }, transaction);
                    }
                }
                var doc = await AddUpdateAssignmentDocs(request.AssignmentDocs, request.AssignmentID);
                transaction.Commit();
                return new ServiceResponse<string>(request.AssignmentID.ToString(), true, "Assignment added/updated successfully.", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding/updating assignment.");
                transaction.Rollback();
                return new ServiceResponse<string>(null, false, "Operation failed: " + ex.Message, 500);
            }
        }
        public async Task<ServiceResponse<List<GetAllAssignmentsResponse>>> GetAllAssignments(GetAllAssignmentsRequest request)
        {
            try
            {
                var sql = @"
            WITH AssignmentData AS (
                SELECT 
                    a.AssignmentID,
                    a.AssignmentName,
                    s.subject_name AS SubjectName,
                    at.AssignmentType,
                    a.Description,
                    a.Reference,
                    a.Attachments,
                    a.StartDate,
                    a.SubmissionDate,
                    a.IsActive,
                    ROW_NUMBER() OVER (ORDER BY a.AssignmentID) AS RowNum
                FROM tblAssignment a
                INNER JOIN tbl_InstituteSubjects s ON a.SubjectID = s.institute_subject_id
                INNER JOIN tblAssignmentType at ON a.AssignmentTypeID = at.AssignmentTypeID
                WHERE a.InstituteID = @InstituteID 
                    AND a.IsActive = 1
                    AND a.StartDate >= @StartDate
                    AND a.SubmissionDate <= @EndDate
                    AND (a.AssignmentTypeID = @AssignmentTypeID OR @AssignmentTypeID = 0)  -- Allow all types if ID is 0
            ), ClassSectionData AS (
                SELECT 
                    acs.AssignmentID,
                    c.class_name AS ClassName,
                    sec.section_name AS SectionName
                FROM tblAssignmentClassSection acs
                INNER JOIN tbl_Class c ON acs.ClassID = c.class_id
                INNER JOIN tbl_Section sec ON acs.SectionID = sec.section_id
                WHERE acs.AssignmentID IN (SELECT AssignmentID FROM AssignmentData WHERE RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize)
            )
            SELECT 
                ad.AssignmentID,
                ad.AssignmentName,
                ad.SubjectName,
                ad.AssignmentType,
                ad.Description,
                ad.Reference,
                ad.Attachments,
                ad.StartDate,
                ad.SubmissionDate,
                ad.IsActive,
                cs.ClassName,
                cs.SectionName
            FROM AssignmentData ad
            LEFT JOIN ClassSectionData cs ON ad.AssignmentID = cs.AssignmentID
            WHERE ad.RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize;";

                using var multi = await _dbConnection.QueryMultipleAsync(sql, new
                {
                    request.InstituteID,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    AssignmentTypeID = request.AssignmentTypeID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                var assignmentList = new List<GetAllAssignmentsResponse>();
                var assignmentData = await multi.ReadAsync<dynamic>();
                var groupedData = assignmentData.GroupBy(ad => new
                {
                    ad.AssignmentID,
                    ad.AssignmentName,
                    ad.SubjectName,
                    ad.AssignmentType,
                    ad.Description,
                    ad.Reference,
                    ad.Attachments,
                    ad.StartDate,
                    ad.SubmissionDate,
                    ad.IsActive
                });

                foreach (var group in groupedData)
                {
                    var assignmentResponse = new GetAllAssignmentsResponse
                    {
                        AssignmentID = group.Key.AssignmentID,
                        AssignmentName = group.Key.AssignmentName,
                        SubjectName = group.Key.SubjectName,
                        AssignmentType = group.Key.AssignmentType,
                        Description = group.Key.Description,
                        Reference = group.Key.Reference,
                        Attachments = group.Key.Attachments,
                        StartDate = group.Key.StartDate,
                        SubmissionDate = group.Key.SubmissionDate,
                        IsActive = group.Key.IsActive,
                        ClassSections = group.Select(g => new ClassSectionResponse
                        {
                            AssignmentID = g.AssignmentID,
                            ClassName = g.ClassName,
                            SectionName = g.SectionName
                        }).ToList()
                    };

                    assignmentList.Add(assignmentResponse);
                }

                // Fetch assignment documents
                foreach (var data in assignmentList)
                {
                    data.AssignmentDocs =  GetAssignmentDocuments(data.AssignmentID); // Make sure to await this call
                }

                return new ServiceResponse<List<GetAllAssignmentsResponse>>(assignmentList, true, "Assignments retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving assignments.");
                return new ServiceResponse<List<GetAllAssignmentsResponse>>(null, false, "Operation failed: " + ex.Message, 500);
            }
        }

        //public async Task<ServiceResponse<List<GetAllAssignmentsResponse>>> GetAllAssignments(GetAllAssignmentsRequest request)
        //{
        //    try
        //    {
        //        var sql = @"
        //            WITH AssignmentData AS (
        //                SELECT 
        //                    a.AssignmentID,
        //                    a.AssignmentName,
        //                    s.subject_name AS SubjectName,
        //                    at.AssignmentType,
        //                    a.Description,
        //                    a.Reference,
        //                    a.Attachments,
        //                    a.StartDate,
        //                    a.SubmissionDate,
        //                    a.IsActive,
        //                    ROW_NUMBER() OVER (ORDER BY a.AssignmentID) AS RowNum
        //                FROM tblAssignment a
        //                INNER JOIN tbl_InstituteSubjects s ON a.SubjectID = s.institute_subject_id
        //                INNER JOIN tblAssignmentType at ON a.AssignmentTypeID = at.AssignmentTypeID
        //                WHERE a.InstituteID = @InstituteID AND a.IsActive = 1
        //            ), ClassSectionData AS (
        //                SELECT 
        //                    acs.AssignmentID,
        //                    c.class_name AS ClassName,
        //                    sec.section_name AS SectionName
        //                FROM tblAssignmentClassSection acs
        //                INNER JOIN tbl_Class c ON acs.ClassID = c.class_id
        //                INNER JOIN tbl_Section sec ON acs.SectionID = sec.section_id
        //                WHERE acs.AssignmentID IN (SELECT AssignmentID FROM AssignmentData WHERE RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize)
        //            )
        //            SELECT 
        //                ad.AssignmentID,
        //                ad.AssignmentName,
        //                ad.SubjectName,
        //                ad.AssignmentType,
        //                ad.Description,
        //                ad.Reference,
        //                ad.Attachments,
        //                ad.StartDate,
        //                ad.SubmissionDate,
        //                ad.IsActive,
        //                cs.ClassName,
        //                cs.SectionName
        //            FROM AssignmentData ad
        //            LEFT JOIN ClassSectionData cs ON ad.AssignmentID = cs.AssignmentID
        //            WHERE ad.RowNum BETWEEN @Offset + 1 AND @Offset + @PageSize;";

        //        using var multi = await _dbConnection.QueryMultipleAsync(sql, new
        //        {
        //            request.InstituteID,
        //            Offset = (request.PageNumber - 1) * request.PageSize,
        //            PageSize = request.PageSize
        //        });

        //        var assignmentList = new List<GetAllAssignmentsResponse>();

        //        var assignmentData = await multi.ReadAsync<dynamic>();
        //        var groupedData = assignmentData.GroupBy(ad => new
        //        {
        //            ad.AssignmentID,
        //            ad.AssignmentName,
        //            ad.SubjectName,
        //            ad.AssignmentType,
        //            ad.Description,
        //            ad.Reference,
        //            ad.Attachments,
        //            ad.StartDate,
        //            ad.SubmissionDate,
        //            ad.IsActive
        //        });

        //        foreach (var group in groupedData)
        //        {
        //            var assignmentResponse = new GetAllAssignmentsResponse
        //            {
        //                AssignmentID = group.Key.AssignmentID,
        //                AssignmentName = group.Key.AssignmentName,
        //                SubjectName = group.Key.SubjectName,
        //                AssignmentType = group.Key.AssignmentType,
        //                Description = group.Key.Description,
        //                Reference = group.Key.Reference,
        //                Attachments = group.Key.Attachments,
        //                StartDate = group.Key.StartDate,
        //                SubmissionDate = group.Key.SubmissionDate,
        //                IsActive = group.Key.IsActive,
        //                ClassSections = group.Select(g => new ClassSectionResponse
        //                {
        //                    AssignmentID = g.AssignmentID,
        //                    ClassName = g.ClassName,
        //                    SectionName = g.SectionName
        //                }).ToList()
        //            };

        //            assignmentList.Add(assignmentResponse);
        //        }
        //        foreach (var data in assignmentList)
        //        {
        //            data.AssignmentDocs = GetAssignmentDocuments(data.AssignmentID);
        //        }
        //        return new ServiceResponse<List<GetAllAssignmentsResponse>>(assignmentList, true, "Assignments retrieved successfully.", 200);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while retrieving assignments.");
        //        return new ServiceResponse<List<GetAllAssignmentsResponse>>(null, false, "Operation failed: " + ex.Message, 500);
        //    }
        //}

        public async Task<ServiceResponse<Assignment>> GetAssignmentById(int id)
        {
            try
            {
                var sql = @"
                    SELECT 
                        a.AssignmentID,
                        a.AssignmentName,
                        a.IsClasswise,
                        a.IsStudentwise,
                        a.ClassID,
                        a.SectionID,
                        a.StudentID,
                        a.SubjectID,
                        s.subject_name AS SubjectName,
                        a.AssignmentTypeID,
                        at.AssignmentType,
                        a.StartDate,
                        a.SubmissionDate,
                        a.Description,
                        a.Reference,
                        a.Attachments,
                        a.InstituteID,
                        a.IsActive
                    FROM tblAssignment a
                    INNER JOIN tbl_InstituteSubjects s ON a.SubjectID = s.institute_subject_id
                    INNER JOIN tblAssignmentType at ON a.AssignmentTypeID = at.AssignmentTypeID
                    WHERE a.AssignmentID = @AssignmentID;

                    SELECT 
                        acs.AssignmentID,
                        acs.ClassID,
                        c.class_name AS ClassName,
                        acs.SectionID,
                        sec.section_name AS SectionName
                    FROM tblAssignmentClassSection acs
                    INNER JOIN tbl_Class c ON acs.ClassID = c.class_id
                    INNER JOIN tbl_Section sec ON acs.SectionID = sec.section_id
                    WHERE acs.AssignmentID = @AssignmentID;";

                using var multi = await _dbConnection.QueryMultipleAsync(sql, new { AssignmentID = id });

                var assignment = await multi.ReadSingleOrDefaultAsync<Assignment>();
                if (assignment != null)
                {
                    assignment.ClassSections = (await multi.ReadAsync<AssignmentClassSection>()).ToList();
                    assignment.AssignmentDocs = GetAssignmentDocuments(id);
                }

                return new ServiceResponse<Assignment>(assignment, assignment != null, assignment != null ? "Assignment found." : "Assignment not found.", assignment != null ? 200 : 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving assignment by ID.");
                return new ServiceResponse<Assignment>(null, false, "Operation failed: " + ex.Message, 500);
            }
        }
        public async Task<ServiceResponse<byte[]>> DownloadDocument(int documentId)
        {
            try
            {
                // Step 1: Fetch the document filename from the database
                string sql = @"SELECT DocFile FROM tblAssignmentDocuments WHERE DocumentsId = @DocumentId";
                var filename = await _dbConnection.QueryFirstOrDefaultAsync<string>(sql, new { DocumentId = documentId });

                if (string.IsNullOrEmpty(filename))
                {
                    return new ServiceResponse<byte[]>([], false, "Document not found", 404);
                }

                // Step 2: Retrieve the file as a Base64 string
                var base64String = GetImage(filename);
                if (string.IsNullOrEmpty(base64String))
                {
                    return new ServiceResponse<byte[]>([], false, "File not found on the server", 404);
                }

                // Step 3: Decode the Base64 string to get the file bytes
                byte[] fileBytes = Convert.FromBase64String(base64String);

                // Return the file bytes in the ServiceResponse
                return new ServiceResponse<byte[]>(fileBytes, true, filename, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>([], false, ex.Message, 500);
            }
        }
        public async Task<ServiceResponse<bool>> DeleteAssignment(int id)
        {
            try
            {
                var sql = @"UPDATE tblAssignment SET IsActive = 0 WHERE AssignmentID = @AssignmentID";
                var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { AssignmentID = id });

                return new ServiceResponse<bool>(rowsAffected > 0, rowsAffected > 0, rowsAffected > 0 ? "Assignment deleted successfully." : "Delete operation failed.", rowsAffected > 0 ? 200 : 400);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting assignment.");
                return new ServiceResponse<bool>(false, false, "Operation failed: " + ex.Message, 500);
            }
        }
        private async Task<ServiceResponse<int>> AddUpdateAssignmentDocs(List<AssignmentDocs> request, int assignmentId)
        {
            if (request == null || !request.Any())
            {
                return new ServiceResponse<int>(0, false, "No documents provided to update", 400);
            }

            try
            {
                // Step 1: Hard delete existing documents for the given AssignmentID
                string deleteSql = @"
        DELETE FROM tblAssignmentDocuments 
        WHERE AssignmentID = @AssignmentID"
                ;

                await _dbConnection.ExecuteAsync(deleteSql, new { AssignmentID = assignmentId });

                // Step 2: Insert new documents
                string insertSql = @"
        INSERT INTO tblAssignmentDocuments (AssignmentID, DocFile)
        VALUES (@AssignmentID, @DocFile)";

                foreach (var doc in request)
                {
                    doc.AssignmentID = assignmentId; // Ensure the AssignmentID is set for each document
                    doc.DocFile = ImageUpload(doc.DocFile); // Assuming ImageUpload handles file processing
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
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "AssignmentDocs");

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
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "AssignmentDocs", Filename);

            if (!File.Exists(filePath))
            {
                return string.Empty;
            }
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }
        private List<AssignmentDocs> GetAssignmentDocuments(int assignmentId)
        {
            try
            {
                string sql = @"
        SELECT DocumentsId, AssignmentID, DocFile 
        FROM tblAssignmentDocuments 
        WHERE AssignmentID = @AssignmentID";

                var documents =  _dbConnection.Query<AssignmentDocs>(sql, new { AssignmentID = assignmentId });

                if (documents != null && documents.Any())
                {
                    return documents.ToList();
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
