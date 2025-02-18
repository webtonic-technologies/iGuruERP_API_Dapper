using System.Data.SqlClient;
using Dapper;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration; 

namespace StudentManagement_API.Repository.Implementations
{
    public class UpdateStudentDocumentRepository : IUpdateStudentDocumentRepository
    {
        private readonly string _connectionString;

        public UpdateStudentDocumentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //public async Task<int> AddDocumentAsync(AddUpdateDocumentRequest request)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    string sql = @"
        //        INSERT INTO tbl_StudentDocumentMaster (Student_Document_Name, en_date, Institute_id, isDelete)
        //        VALUES (@DocumentName, GETDATE(), @InstituteID, 0);
        //        SELECT CAST(SCOPE_IDENTITY() AS int);";
        //    var parameters = new { DocumentName = request.DocumentName, InstituteID = request.InstituteID };
        //    connection.Open();
        //    var id = await connection.QuerySingleAsync<int>(sql, parameters);
        //    return id;
        //}


        public async Task<List<int>> AddDocumentAsync(AddUpdateDocumentRequest request)
        {
            var insertedIds = new List<int>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                string sql = @"
                    INSERT INTO tbl_StudentDocumentMaster (Student_Document_Name, en_date, Institute_id, isDelete)
                    VALUES (@DocumentName, GETDATE(), @InstituteID, 0);
                    SELECT CAST(SCOPE_IDENTITY() AS int);";

                foreach (var docName in request.DocumentNames)
                {
                    var parameters = new { DocumentName = docName, InstituteID = request.InstituteID };
                    var id = await connection.QuerySingleAsync<int>(sql, parameters, transaction);
                    insertedIds.Add(id);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw; // Optionally, handle or log the exception as needed.
            }

            return insertedIds;
        }

        public async Task<IEnumerable<GetDocumentsResponse>> GetDocumentsAsync(GetDocumentsRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            // Using FORMAT to return the date in "dd-MM-yyyy at hh:mm tt" format.
            string sql = @"
                SELECT 
                    Student_Document_id AS DocumentID,
                    Student_Document_Name AS DocumentName,
                    FORMAT(en_date, 'dd-MM-yyyy ''at'' hh:mm tt') AS CreatedDateTime
                FROM tbl_StudentDocumentMaster
                WHERE Institute_id = @InstituteID
                  AND isDelete = 0";

            // If a search string is provided, filter by DocumentName.
            if (!string.IsNullOrEmpty(request.Search))
            {
                sql += " AND Student_Document_Name LIKE '%' + @Search + '%'";
            }

            var documents = await connection.QueryAsync<GetDocumentsResponse>(
                sql,
                new { InstituteID = request.InstituteID, Search = request.Search }
            );
            return documents;
        }

        public async Task<bool> DeleteDocumentAsync(DeleteDocumentRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
                UPDATE tbl_StudentDocumentMaster 
                SET isDelete = 1 
                WHERE Student_Document_id = @DocumentID 
                  AND Institute_id = @InstituteID";
            var affectedRows = await connection.ExecuteAsync(sql, new { DocumentID = request.DocumentID, InstituteID = request.InstituteID });
            return affectedRows > 0;
        }

        public async Task<bool> SetDocumentManagerAsync(List<SetDocumentManagerRequest> requests)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (var studentRequest in requests)
                {
                    foreach (var doc in studentRequest.StudentDocuments)
                    {
                        if (doc.IsSubmitted)
                        {
                            // Insert record if it does not exist.
                            string sqlInsert = @"
                                IF NOT EXISTS (
                                    SELECT 1 FROM tbl_DocManager 
                                    WHERE student_id = @StudentId 
                                      AND document_id = @DocumentID
                                      AND class_id = @ClassID 
                                      AND section_id = @SectionID
                                )
                                BEGIN
                                    INSERT INTO tbl_DocManager (student_id, document_id, class_id, section_id)
                                    VALUES (@StudentId, @DocumentID, @ClassID, @SectionID);
                                END";
                            await connection.ExecuteAsync(
                                sqlInsert,
                                new
                                {
                                    StudentId = studentRequest.StudentId,
                                    doc.DocumentID,
                                    doc.ClassID,
                                    doc.SectionID
                                },
                                transaction);
                        }
                        else
                        {
                            // Delete record if it exists.
                            string sqlDelete = @"
                                DELETE FROM tbl_DocManager 
                                WHERE student_id = @StudentId 
                                  AND document_id = @DocumentID
                                  AND class_id = @ClassID 
                                  AND section_id = @SectionID";
                            await connection.ExecuteAsync(
                                sqlDelete,
                                new
                                {
                                    StudentId = studentRequest.StudentId,
                                    doc.DocumentID,
                                    doc.ClassID,
                                    doc.SectionID
                                },
                                transaction);
                        }
                    }
                }
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw; // Optionally log the exception.
            }
        }

        private class StudentDocManagerFlat
        {
            public int StudentID { get; set; }
            public string StudentName { get; set; }
            public string Admission_Number { get; set; }
            public string Class { get; set; }
            public string Section { get; set; }
            public int DocumentID { get; set; }
            public string DocumentName { get; set; }
            public bool IsSubmitted { get; set; }
        }

        public async Task<IEnumerable<GetDocumentManagerResponse>> GetDocumentManagerAsync(GetDocumentManagerRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
                SELECT 
                    s.student_id AS StudentID,
                    CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name, ''), ' ', s.Last_Name) AS StudentName,
                    s.Admission_Number,
                    c.class_name AS Class,
                    sec.section_name AS Section,
                    dsm.Student_Document_id AS DocumentID,
                    dsm.Student_Document_Name AS DocumentName,
                    CASE 
                        WHEN dm.document_id IS NULL THEN CAST(0 AS bit) 
                        ELSE CAST(1 AS bit) 
                    END AS IsSubmitted
                FROM tbl_StudentMaster s
                INNER JOIN tbl_class c 
                    ON s.class_id = c.class_id
                INNER JOIN tbl_Section sec 
                    ON s.section_id = sec.section_id
                CROSS JOIN tbl_StudentDocumentMaster dsm
                LEFT JOIN tbl_DocManager dm 
                    ON dm.student_id = s.student_id
                    AND dm.document_id = dsm.Student_Document_id
                    AND dm.class_id = @ClassID 
                    AND dm.section_id = @SectionID
                WHERE s.Institute_id = @InstituteID
                  AND s.class_id = @ClassID
                  AND s.section_id = @SectionID
                  AND dsm.isDelete = 0
                ORDER BY s.student_id, dsm.Student_Document_id;
            ";

            var flatList = await connection.QueryAsync<StudentDocManagerFlat>(sql, new { request.InstituteID, request.ClassID, request.SectionID });

            // Group the flat results by student
            var grouped = flatList.GroupBy(x => new { x.StudentID, x.StudentName, x.Admission_Number, x.Class, x.Section })
                .Select(g => new GetDocumentManagerResponse
                {
                    StudentID = g.Key.StudentID,
                    StudentName = g.Key.StudentName,
                    AdmissionNumber = g.Key.Admission_Number,
                    Class = g.Key.Class,
                    Section = g.Key.Section,
                    Documents = g.Select(d => new DocumentDetailResponse
                    {
                        DocumentID = d.DocumentID,
                        DocumentName = d.DocumentName,
                        IsSubmitted = d.IsSubmitted
                    }).ToList()
                });

            return grouped;
        }

        private class StudentDocManagerFlatExport
        {
            public int StudentID { get; set; }
            public string StudentName { get; set; }
            public string Admission_Number { get; set; }
            public string Class { get; set; }
            public string Section { get; set; }
            public int DocumentID { get; set; }
            public string DocumentName { get; set; }
            public bool IsSubmitted { get; set; }
        }

        public async Task<IEnumerable<GetDocumentManagerExportResponse>> GetDocumentManagerExportAsync(GetDocumentManagerExportRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
                SELECT 
                    s.student_id AS StudentID,
                    CONCAT(s.First_Name, ' ', ISNULL(s.Middle_Name, ''), ' ', s.Last_Name) AS StudentName,
                    s.Admission_Number,
                    c.class_name AS Class,
                    sec.section_name AS Section,
                    dsm.Student_Document_id AS DocumentID,
                    dsm.Student_Document_Name AS DocumentName,
                    CASE 
                        WHEN dm.document_id IS NULL THEN CAST(0 AS bit) 
                        ELSE CAST(1 AS bit) 
                    END AS IsSubmitted
                FROM tbl_StudentMaster s
                INNER JOIN tbl_class c 
                    ON s.class_id = c.class_id
                INNER JOIN tbl_Section sec 
                    ON s.section_id = sec.section_id
                CROSS JOIN tbl_StudentDocumentMaster dsm
                LEFT JOIN tbl_DocManager dm 
                    ON dm.student_id = s.student_id
                    AND dm.document_id = dsm.Student_Document_id
                    AND dm.class_id = @ClassID 
                    AND dm.section_id = @SectionID
                WHERE s.Institute_id = @InstituteID
                  AND s.class_id = @ClassID
                  AND s.section_id = @SectionID
                  AND dsm.isDelete = 0
                ORDER BY s.student_id, dsm.Student_Document_id;
            ";

            var flatResults = await connection.QueryAsync<StudentDocManagerFlatExport>(
                sql,
                new { InstituteID = request.InstituteID, ClassID = request.ClassID, SectionID = request.SectionID }
            );

            var grouped = flatResults.GroupBy(x => new { x.StudentID, x.StudentName, x.Admission_Number, x.Class, x.Section })
                .Select(g => new GetDocumentManagerExportResponse
                {
                    StudentID = g.Key.StudentID,
                    StudentName = g.Key.StudentName,
                    AdmissionNumber = g.Key.Admission_Number,
                    Class = g.Key.Class,
                    Section = g.Key.Section,
                    Documents = g.Select(d => new DocumentDetailExportResponse
                    {
                        DocumentID = d.DocumentID,
                        DocumentName = d.DocumentName,
                        IsSubmitted = d.IsSubmitted
                    }).ToList()
                });

            return grouped;
        }
    }
}
