using Dapper;
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using System.Data;

namespace Student_API.Repository.Implementations
{
    public class DocumentManagerRepository : IDocumentManagerRepository
    {
        private readonly IDbConnection _connection;

        public DocumentManagerRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<StudentDocumentInfo>>> GetStudentDocuments(int classId, int sectionId, int? pageSize, int? pageNumber)
        {
            try
            {
                var studentDocuments = new List<StudentDocumentInfo>();
                var studentDocumentDict = new Dictionary<int, StudentDocumentInfo>();

                string query = @"
                SELECT 
                    s.Student_id,
                    s.First_Name AS StudentName,
                    s.Admission_Number,
                    c.class_course AS Class_Name,
                    sec.Section AS Section_Name,
                    doc.Student_Document_id,
                    doc.Student_Document_Name,
                    CASE WHEN dm.document_id IS NOT NULL THEN 1 ELSE 0 END AS IsSubmitted
                FROM 
                    tbl_StudentMaster s
                JOIN 
                    tbl_CourseClass c ON s.Class_id = c.CourseClass_id
                JOIN 
                    tbl_CourseClassSection sec ON s.Section_id = sec.CourseClassSection_id
                LEFT JOIN 
                    tbl_StudentDocumentMaster doc ON doc.Student_Document_id IS NOT NULL
                LEFT JOIN 
                    tbl_DocManager dm ON s.Student_id = dm.student_id AND dm.document_id = doc.Student_Document_id
                WHERE 
                    s.Class_id = @ClassId AND s.Section_id = @SectionId
                ORDER BY 
                    c.class_course, sec.Section, s.First_Name";

                // Pagination logic
                if (pageSize.HasValue && pageNumber.HasValue)
                {
                    int offset = (pageNumber.Value - 1) * pageSize.Value;
                    query += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    query += "; SELECT COUNT(*) FROM tbl_StudentMaster s WHERE s.Class_id = @ClassId AND s.Section_id = @SectionId";
                }

                var multi = await _connection.QueryMultipleAsync(query, new
                {
                    ClassId = classId,
                    SectionId = sectionId,
                    PageSize = pageSize,
                    Offset = pageNumber.HasValue ? (pageNumber.Value - 1) * pageSize : (int?)null
                });

                var results = multi.Read().ToList();

                foreach (var row in results)
                {
                    int studentId = row.Student_id;
                    int documentId = row.Student_Document_id;
                    string documentName = row.Student_Document_Name;
                    bool isSubmitted = row.IsSubmitted == 1;

                    if (!studentDocumentDict.ContainsKey(studentId))
                    {
                        studentDocumentDict[studentId] = new StudentDocumentInfo
                        {
                            Student_id = studentId,
                            StudentName = row.Student_Name,
                            Admission_Number = row.Admission_Number,
                            Class_Name = row.Class_Name,
                            Section_Name = row.Section_Name,
                            DocumentStatus = new Dictionary<string, DocumentStatusInfo>()
                        };
                    }

                    studentDocumentDict[studentId].DocumentStatus[documentName] = new DocumentStatusInfo
                    {
                        Student_Document_id = documentId,
                        IsSubmitted = isSubmitted
                    };
                }

                studentDocuments = studentDocumentDict.Values.ToList();

                var response = new ServiceResponse<List<StudentDocumentInfo>>(true, "Student documents retrieved successfully", studentDocuments, 200);

                // Add total count if pagination is applied
                if (pageSize.HasValue && pageNumber.HasValue)
                {
                    var totalCount = multi.ReadSingle<int>();
                    response.TotalCount = totalCount;
                }

                return response;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentDocumentInfo>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateStudentDocumentStatuses(List<DocumentUpdateRequest> updates)
        {
            using var transaction = _connection.BeginTransaction();

            try
            {
                foreach (var update in updates)
                {
                    if (update.IsSubmitted)
                    {
                        // Insert or update record
                        string insertOrUpdateQuery = @"
                        IF EXISTS (SELECT 1 FROM tbl_DocManager WHERE student_id = @StudentId AND document_id = @DocumentId)
                        BEGIN
                            UPDATE tbl_DocManager
                            SET class_id = @ClassId, section_id = @SectionId
                            WHERE student_id = @StudentId AND document_id = @DocumentId
                        END
                        ELSE
                        BEGIN
                            INSERT INTO tbl_DocManager (student_id, document_id, class_id, section_id)
                            VALUES (@StudentId, @DocumentId, @ClassId, @SectionId)
                        END";

                        await _connection.ExecuteAsync(insertOrUpdateQuery, new
                        {
                            update.StudentId,
                            update.DocumentId,
                            ClassId = update.ClassId,
                            SectionId = update.SectionId
                        }, transaction);
                    }
                    else
                    {
                        // Delete record if unchecked
                        string deleteQuery = @"
                        DELETE FROM tbl_DocManager
                        WHERE student_id = @StudentId AND document_id = @DocumentId";

                        await _connection.ExecuteAsync(deleteQuery, new
                        {
                            update.StudentId,
                            update.DocumentId
                        }, transaction);
                    }
                }

                transaction.Commit();
                return new ServiceResponse<bool>(true, "Student document statuses updated successfully", true, 200);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
