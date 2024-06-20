using Dapper;
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using System.Data;

namespace Student_API.Repository.Implementations
{
    public class DocumentConfigRepository : IDocumentConfigRepository
    {
        private readonly IDbConnection _connection;

        public DocumentConfigRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<ServiceResponse<int>> AddUpdateStudentDocument(StudentDocumentConfigDTO studentDocumentDto)
        {
            try
            {

                try
                {
                    string query;
                    if (studentDocumentDto.Student_Document_id > 0)
                    {
                        query = @"
                    UPDATE [dbo].[tbl_StudentDocumentMaster]
                    SET Student_Document_Name = @Student_Document_Name,
                        en_date = @en_date
                    WHERE Student_Document_id = @Student_Document_id";
                    }
                    else
                    {
                        query = @"
                    INSERT INTO [dbo].[tbl_StudentDocumentMaster] (Student_Document_Name, en_date)
                    VALUES (@Student_Document_Name, @en_date);
                    SELECT SCOPE_IDENTITY();";
                    }

                    int id = await _connection.ExecuteScalarAsync<int>(query, studentDocumentDto);


                    return new ServiceResponse<int>(true, "Student document config saved successfully", id, 200);
                }
                catch (Exception ex)
                {

                    return new ServiceResponse<int>(false, ex.Message, 0, 500);
                }
            }

            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<StudentDocumentConfigDTO>> GetStudentDocumentConfigById(int DocumentConfigtId)
        {
            try
            {
                string query = @"
        SELECT Student_Document_id, Student_Document_Name, en_date
        FROM [dbo].[tbl_StudentDocumentMaster]
        WHERE Student_Document_id = @DocumentConfigtId";

                var studentDocument = await _connection.QuerySingleOrDefaultAsync<StudentDocumentConfigDTO>(query, new { DocumentConfigtId });

                if (studentDocument == null)
                {
                    return new ServiceResponse<StudentDocumentConfigDTO>(false, "Student document not found", null, 404);
                }

                return new ServiceResponse<StudentDocumentConfigDTO>(true, "Student document retrieved successfully", studentDocument, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<StudentDocumentConfigDTO>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<bool>> DeleteStudentDocument(int studentDocumentId)
        {
            try
            {

                try
                {
                    string query = @"
                DELETE FROM [dbo].[tbl_StudentDocumentMaster]
                WHERE Student_Document_id = @studentDocumentId";

                    await _connection.ExecuteAsync(query, new { studentDocumentId });

                    return new ServiceResponse<bool>(true, "Student document deleted successfully", true, 200);
                }
                catch (Exception ex)
                {

                    return new ServiceResponse<bool>(false, ex.Message, false, 500);
                }
            }

            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        public async Task<ServiceResponse<List<StudentDocumentConfigDTO>>> GetAllStudentDocuments(string sortColumn, string sortDirection, int? pageSize = null, int? pageNumber = null)
        {
            try
            {
                // List of valid sortable columns
                var validSortColumns = new Dictionary<string, string>
        {
            { "Document_Name", "Student_Document_Name" },
            { "Created", "en_date" }
        };

                // Ensure the sort column is valid, default to "Student_Document_Name" if not
                if (!validSortColumns.ContainsKey(sortColumn))
                {
                    sortColumn = "Student_Document_Name";
                }
                else
                {
                    sortColumn = validSortColumns[sortColumn];
                }

                // Ensure sort direction is valid, default to "ASC" if not
                sortDirection = sortDirection.ToUpper() == "DESC" ? "DESC" : "ASC";

                // SQL queries
                string queryAll = @"
            SELECT Student_Document_id, Student_Document_Name, en_date
            FROM [dbo].[tbl_StudentDocumentMaster]";

                string queryCount = @"
            SELECT COUNT(*)
            FROM [dbo].[tbl_StudentDocumentMaster]";

                List<StudentDocumentConfigDTO> studentDocuments;
                int totalRecords = 0;

                if (pageSize.HasValue && pageNumber.HasValue)
                {
                    int offset = (pageNumber.Value - 1) * pageSize.Value;

                    // Build the paginated query with dynamic sorting
                    string queryPaginated = $@"
                {queryAll}
                ORDER BY {sortColumn} {sortDirection}
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                {queryCount}";

                    using (var multi = await _connection.QueryMultipleAsync(queryPaginated, new { Offset = offset, PageSize = pageSize }))
                    {
                        studentDocuments = multi.Read<StudentDocumentConfigDTO>().ToList();
                        totalRecords = multi.ReadSingle<int>();
                    }

                    return new ServiceResponse<List<StudentDocumentConfigDTO>>(true, "Student documents retrieved successfully", studentDocuments, 200, totalRecords);
                }
                else
                {
                    // No pagination, return all records with sorting
                    string querySorted = $@"
                {queryAll}
                ORDER BY {sortColumn} {sortDirection}";

                    studentDocuments = (await _connection.QueryAsync<StudentDocumentConfigDTO>(querySorted)).ToList();
                    return new ServiceResponse<List<StudentDocumentConfigDTO>>(true, "All student documents retrieved successfully", studentDocuments, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentDocumentConfigDTO>>(false, ex.Message, null, 500);
            }
        }


    }
}
