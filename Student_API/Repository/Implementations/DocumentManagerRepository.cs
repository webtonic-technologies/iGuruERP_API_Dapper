using Dapper;
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Models;
using Student_API.Repository.Interfaces;
using System.Data;
using System.Reflection.Metadata;

namespace Student_API.Repository.Implementations
{
    public class DocumentManagerRepository : IDocumentManagerRepository
    {
        private readonly IDbConnection _connection;

        public DocumentManagerRepository(IDbConnection connection)
        {
            _connection = connection;
        }



        public async Task<ServiceResponse<List<StudentDocumentInfo>>> GetStudentDocuments(
    int Institute_id,
    int classId,
    int sectionId,
    string sortColumn,
    string sortDirection,
    int? pageSize,
    int? pageNumber,
    string? searchQuery)
        {
            try
            {
                // Dictionary to map user-friendly sort columns to database columns
                var validSortColumns = new Dictionary<string, string>
        {
            { "Student_Name", "Student_Name" },
            { "Admission_Number", "Admission_Number" }
        };

                // Ensure the sort column is valid, default to "Student_Name" if not
                if (!validSortColumns.ContainsKey(sortColumn))
                {
                    sortColumn = "Student_Name";
                }
                else
                {
                    sortColumn = validSortColumns[sortColumn];
                }

                // Ensure sort direction is valid, default to "ASC" if not
                sortDirection = sortDirection.ToUpper() == "DESC" ? "DESC" : "ASC";

                // Base query
                string query = @"
            DROP TABLE IF EXISTS #StudentTempTable;

            SELECT 
                s.Student_id,
                CONCAT(s.first_name, ' ', s.last_name) AS Student_Name,
                s.Admission_Number,
                c.Class_Name,
                sec.Section_Name,
                doc.Student_Document_id,
                doc.Student_Document_Name,
                CASE WHEN dm.document_id IS NOT NULL THEN 1 ELSE 0 END AS IsSubmitted
            INTO 
                #StudentTempTable
            FROM 
                tbl_StudentMaster s
            JOIN 
                tbl_class c ON s.Class_id = c.Class_id
            JOIN 
                tbl_Section sec ON s.Section_id = sec.Section_id
            LEFT JOIN 
                tbl_StudentDocumentMaster doc ON doc.Student_Document_id IS NOT NULL
            LEFT JOIN 
                tbl_DocManager dm ON s.Student_id = dm.student_id AND dm.document_id = doc.Student_Document_id
            WHERE 
                s.Institute_id = @Institute_id 
                AND (s.Class_id = @ClassId OR @ClassId = 0) 
                AND (s.Section_id = @SectionId OR @SectionId = 0)
                AND s.isActive = 1";

                // If searchQuery is provided, add LIKE conditions for Student_Name and Admission_Number
                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    query += @"
                AND (s.first_name LIKE '%' + @SearchQuery + '%' OR s.last_name LIKE '%' + @SearchQuery + '%' OR s.Admission_Number LIKE '%' + @SearchQuery + '%')";
                }

                // Debugging: Log the query to ensure the WHERE condition is being correctly added
                Console.WriteLine("Generated Query: " + query);

                // Continue query to fetch sorted results
                query += @"
            SELECT 
                Student_id,
                Student_Name,
                Admission_Number,
                Class_Name,
                Section_Name,
                (SELECT 
                    Student_Document_Name,
                    Student_Document_id,
                    IsSubmitted
                 FROM #StudentTempTable AS doc
                 WHERE doc.Student_id = s.Student_id
                 FOR JSON PATH) AS documentStatus
            FROM 
                #StudentTempTable s
            GROUP BY 
                Student_id, Student_Name, Admission_Number, Class_Name, Section_Name
            ORDER BY 
                " + sortColumn + " " + sortDirection + @"
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROWS ONLY;

            -- Fetch total count of distinct students
            SELECT 
                COUNT(DISTINCT Student_id)
            FROM 
                tbl_StudentMaster
            WHERE 
                Institute_id = @Institute_id 
                AND (Class_id = @ClassId OR @ClassId = 0)
                AND (Section_id = @SectionId OR @SectionId = 0)
                AND isActive = 1 AND (first_name LIKE '%' + @SearchQuery + '%' OR last_name LIKE '%' + @SearchQuery + '%' OR Admission_Number LIKE '%' + @SearchQuery + '%')"; 

                // Parameters for query execution
                var parameters = new
                {
                    ClassId = classId,
                    SectionId = sectionId,
                    PageSize = pageSize ?? 12, // Default page size
                    Offset = (pageNumber.HasValue ? (pageNumber.Value - 1) * (pageSize ?? 12) : 0), // Offset calculation
                    Institute_id = Institute_id,
                    SearchQuery = searchQuery // Search query for filtering
                };

                // Execute query using Dapper
                using (var multi = await _connection.QueryMultipleAsync(query, parameters))
                {
                    var studentDocuments = new List<StudentDocumentInfo>();
                    var studentDocumentDict = new Dictionary<int, StudentDocumentInfo>();

                    // Read data from the temporary table
                    var results = multi.Read().ToList();

                    foreach (var row in results)
                    {
                        int studentId = row.Student_id;
                        string studentName = row.Student_Name;
                        string admissionNumber = row.Admission_Number;
                        string className = row.Class_Name;
                        string sectionName = row.Section_Name;

                        if (!studentDocumentDict.ContainsKey(studentId))
                        {
                            studentDocumentDict[studentId] = new StudentDocumentInfo
                            {
                                Student_id = studentId,
                                Student_Name = studentName,
                                Admission_Number = admissionNumber,
                                Class_Name = className,
                                Section_Name = sectionName,
                                DocumentStatus = new Dictionary<string, DocumentStatusInfo>()
                            };
                        }

                        // Deserialize documentStatus JSON into the list
                        var documentStatusList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DocumentStatusInfo>>(row.documentStatus.ToString());

                        foreach (var document in documentStatusList)
                        {
                            studentDocumentDict[studentId].DocumentStatus[document.Student_Document_Name] = document;
                        }
                    }

                    studentDocuments = studentDocumentDict.Values.ToList();

                    var response = new ServiceResponse<List<StudentDocumentInfo>>(true, "Student documents retrieved successfully", studentDocuments, 200);

                    // Add total count based on the total number of students
                    var totalCount = multi.ReadSingle<int>();
                    response.TotalCount = totalCount;

                    return response;
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentDocumentInfo>>(false, ex.Message, null, 500);
            }
        }



        //public async Task<ServiceResponse<List<StudentDocumentInfo>>> GetStudentDocuments(int Institute_id, int classId, int sectionId, string sortColumn, string sortDirection, int? pageSize, int? pageNumber)
        //{
        //    try
        //    {
        //        // Dictionary to map user-friendly sort columns to database columns
        //        var validSortColumns = new Dictionary<string, string>
        //{
        //    { "Student_Name", "Student_Name" },
        //    { "Admission_Number", "Admission_Number" }
        //};

        //        // Ensure the sort column is valid, default to "Student_Name" if not
        //        if (!validSortColumns.ContainsKey(sortColumn))
        //        {
        //            sortColumn = "Student_Name";
        //        }
        //        else
        //        {
        //            sortColumn = validSortColumns[sortColumn];
        //        }

        //        // Ensure sort direction is valid, default to "ASC" if not
        //        sortDirection = sortDirection.ToUpper() == "DESC" ? "DESC" : "ASC";

        //        // Query to select data into temporary table
        //        string query = $@"
        //    DROP TABLE IF EXISTS #StudentTempTable;

        //    SELECT 
        //        s.Student_id,
        //        CONCAT(s.first_name, ' ', s.last_name) AS Student_Name,
        //        s.Admission_Number,
        //        c.Class_Name,
        //        sec.Section_Name ,
        //        doc.Student_Document_id,
        //        doc.Student_Document_Name,
        //        CASE WHEN dm.document_id IS NOT NULL THEN 1 ELSE 0 END AS IsSubmitted
        //    INTO 
        //        #StudentTempTable
        //    FROM 
        //        tbl_StudentMaster s
        //    JOIN 
        //        tbl_class c ON s.Class_id = c.Class_id
        //    JOIN 
        //        tbl_Section sec ON s.Section_id = sec.Section_id
        //    LEFT JOIN 
        //        tbl_StudentDocumentMaster doc ON doc.Student_Document_id IS NOT NULL
        //    LEFT JOIN 
        //        tbl_DocManager dm ON s.Student_id = dm.student_id AND dm.document_id = doc.Student_Document_id
        //    WHERE 
        //        s.Institute_id = @Institute_id AND (s.Class_id = @ClassId OR @ClassId = 0) AND (s.Section_id = @SectionId OR @SectionId = 0)
        //        AND s.isActive = 1;

        //    -- Fetch sorted results based on students
        //    SELECT 
        //        Student_id,
        //        Student_Name,
        //        Admission_Number,
        //        Class_Name,
        //        Section_Name,
        //        (SELECT 
        //            Student_Document_Name,
        //            Student_Document_id,
        //            IsSubmitted
        //         FROM #StudentTempTable AS doc
        //         WHERE doc.Student_id = s.Student_id
        //         FOR JSON PATH) AS documentStatus
        //    FROM 
        //        #StudentTempTable s
        //    GROUP BY 
        //        Student_id, Student_Name, Admission_Number, Class_Name, Section_Name
        //    ORDER BY 
        //        {sortColumn} {sortDirection}
        //    OFFSET @Offset ROWS 
        //    FETCH NEXT @PageSize ROWS ONLY;

        //    -- Fetch total count of distinct students
        //    SELECT 
        //        COUNT(DISTINCT Student_id)
        //    FROM 
        //        tbl_StudentMaster
        //    WHERE 
        //        Institute_id = @Institute_id 
        //        AND (Class_id = @ClassId OR @ClassId = 0)
        //        AND (Section_id = @SectionId OR @SectionId = 0)
        //        AND isActive = 1;

        //    DROP TABLE IF EXISTS #StudentTempTable;";

        //        var parameters = new
        //        {
        //            ClassId = classId,
        //            SectionId = sectionId,
        //            PageSize = pageSize ?? 12, // Max value for page size if not specified
        //            Offset = (pageNumber.HasValue ? (pageNumber.Value - 1) * (pageSize ?? 12) : 0), // Offset calculation for pagination
        //            Institute_id = Institute_id
        //        };

        //        // Fetch data using Dapper
        //        using (var multi = await _connection.QueryMultipleAsync(query, parameters))
        //        {
        //            var studentDocuments = new List<StudentDocumentInfo>();
        //            var studentDocumentDict = new Dictionary<int, StudentDocumentInfo>();

        //            // Read data from the temporary table
        //            var results = multi.Read().ToList();

        //            foreach (var row in results)
        //            {
        //                int studentId = row.Student_id;
        //                string studentName = row.Student_Name;
        //                string admissionNumber = row.Admission_Number;
        //                string className = row.Class_Name;
        //                string sectionName = row.Section_Name;

        //                if (!studentDocumentDict.ContainsKey(studentId))
        //                {
        //                    studentDocumentDict[studentId] = new StudentDocumentInfo
        //                    {
        //                        Student_id = studentId,
        //                        Student_Name = studentName,
        //                        Admission_Number = admissionNumber,
        //                        Class_Name = className,
        //                        Section_Name = sectionName,
        //                        DocumentStatus = new Dictionary<string, DocumentStatusInfo>()
        //                    };
        //                }

        //                // Ensure documentStatus is correctly deserialized into a list of document status
        //                var documentStatusList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DocumentStatusInfo>>(row.documentStatus.ToString());

        //                foreach (var document in documentStatusList)
        //                {
        //                    studentDocumentDict[studentId].DocumentStatus[document.Student_Document_Name] = document;
        //                }
        //            }

        //            studentDocuments = studentDocumentDict.Values.ToList();

        //            var response = new ServiceResponse<List<StudentDocumentInfo>>(true, "Student documents retrieved successfully", studentDocuments, 200);

        //            // Add total count based on the total number of students
        //            var totalCount = multi.ReadSingle<int>();
        //            response.TotalCount = totalCount;

        //            return response;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<List<StudentDocumentInfo>>(false, ex.Message, null, 500);
        //    }
        //}



        public async Task<ServiceResponse<bool>> UpdateStudentDocumentStatuses(List<DocumentUpdateRequest> updateList)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            using var transaction = _connection.BeginTransaction();
            {
                try
                {
                    foreach (var document in updateList)
                    {
                        foreach (var update in document.StudentDocuments)
                        {
                            if (update.IsSubmitted)
                            {
                                // Insert or update record

                                //    IF EXISTS(SELECT 1 FROM tbl_DocManager WHERE student_id = @StudentId AND document_id = @DocumentId)
                                //BEGIN
                                //    UPDATE tbl_DocManager
                                //    SET class_id = @ClassId, section_id = @SectionId
                                //    WHERE student_id = @StudentId AND document_id = @DocumentId
                                //END
                                //ELSE
                                string insertOrUpdateQuery = @"
                       IF NOT EXISTS(SELECT 1 FROM tbl_DocManager WHERE student_id = @StudentId AND document_id = @DocumentId)
                        BEGIN
                            INSERT INTO tbl_DocManager (student_id, document_id, class_id, section_id)
                            VALUES (@StudentId, @DocumentId, @ClassId, @SectionId)
                        END";

                                await _connection.ExecuteAsync(insertOrUpdateQuery, new
                                {
                                    document.StudentId,
                                    update.DocumentId,
                                    ClassId = update.ClassId,
                                    SectionId = update.SectionId,
                                    //Institute_id = update.Institute_id
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
                                    document.StudentId,
                                    update.DocumentId
                                }, transaction);
                            }
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
}
