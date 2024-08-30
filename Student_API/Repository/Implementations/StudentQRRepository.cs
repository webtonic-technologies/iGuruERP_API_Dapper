using Dapper;
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using System.Data;

namespace Student_API.Repository.Implementations
{
    public class StudentQRRepository : IStudentQRRepository
    {
        private readonly IDbConnection _connection;

        public StudentQRRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<ServiceResponse<List<StudentQRDTO>>> GetAllStudentQR(int sectionId, int classId, string sortField , string sortDirection, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                // Define constants for the maximum page size and validate inputs
                const int MaxPageSize = int.MaxValue;
                int actualPageSize = pageSize ?? MaxPageSize;
                int actualPageNumber = pageNumber ?? 1;
                int offset = (actualPageNumber - 1) * actualPageSize;

                // List of valid sortable columns
                var validSortFields = new List<string> { "Student_Name", "class_course", "Section", "Admission_Number", "Roll_Number" };
                var validSortDirections = new List<string> { "ASC", "DESC" };

                // Default to "Student_Name" if an invalid sort field is provided
                if (!validSortFields.Contains(sortField))
                {
                    sortField = "Student_Name";
                }

                // Default to "ASC" if an invalid sort direction is provided
                sortDirection = sortDirection.ToUpper();
                if (!validSortDirections.Contains(sortDirection))
                {
                    sortDirection = "ASC";
                }

                // Construct the SQL query
                string sql = $@"
            -- Drop temp table if it exists
            IF OBJECT_ID('tempdb..#StudentTempTable') IS NOT NULL
                DROP TABLE #StudentTempTable;

            -- Create temp table and insert data
            SELECT 
                tbl_StudentMaster.student_id,  
                CONCAT(tbl_StudentMaster.first_name, ' ', tbl_StudentMaster.last_name) AS Student_Name, 
               class_name AS  class_course, 
                Section_name AS Section, 
                Admission_Number, 
                Roll_Number, 
                QR_code 
            INTO 
                #StudentTempTable
            FROM 
                [dbo].[tbl_StudentMaster]
            INNER JOIN 
                tbl_Class ON tbl_StudentMaster.class_id = tbl_Class.class_id
            INNER JOIN 
                tbl_Section ON tbl_StudentMaster.section_id = tbl_Section.section_id
            WHERE 
                (@classId  = 0 OR tbl_StudentMaster.class_id = @classId )
                AND (@sectionId = 0 OR tbl_StudentMaster.section_id = @sectionId);

            -- Select sorted and paginated data
            SELECT * 
            FROM #StudentTempTable
            ORDER BY {sortField} {sortDirection}, student_id
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;

            -- Count total records
            SELECT COUNT(1) 
            FROM #StudentTempTable;

            -- Drop temp table
            IF OBJECT_ID('tempdb..#StudentTempTable') IS NOT NULL
                DROP TABLE #StudentTempTable;
        ";

                // Execute the query with Dapper
                using (var multi = await _connection.QueryMultipleAsync(sql, new { classId, sectionId, Offset = offset, PageSize = actualPageSize }))
                {
                    // Read student list and total record count
                    var studentList = multi.Read<StudentQRDTO>().ToList();
                    int? totalRecords = pageSize.HasValue && pageNumber.HasValue ? multi.ReadSingle<int>() : null;

                    // Return response based on the student list
                    if (studentList.Any())
                    {
                        return new ServiceResponse<List<StudentQRDTO>>(true, "Operation successful", studentList, 200, totalRecords);
                    }
                    else
                    {
                        return new ServiceResponse<List<StudentQRDTO>>(true, "Student not found", null, 404);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentQRDTO>>(false, ex.Message, null, 500);
            }
        }


    }
}
