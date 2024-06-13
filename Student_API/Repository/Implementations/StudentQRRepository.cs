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
        public async Task<ServiceResponse<List<StudentQRDTO>>> GetAllStudentQR(int sectionId, int classId, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                // Define a constant for the maximum number of records to fetch when no pagination is specified
                const int MaxPageSize = int.MaxValue;

                // Set default values if pageSize or pageNumber are not provided
                int actualPageSize = pageSize ?? MaxPageSize;
                int actualPageNumber = pageNumber ?? 1;
                int offset = (actualPageNumber - 1) * actualPageSize;

                string sql = @"
                    DROP TABLE IF EXISTS #StudentTempTable;
                    SELECT tbl_StudentMaster.student_id, tbl_StudentMaster.First_Name, tbl_StudentMaster.Last_Name, class_course, Section, Admission_Number, Roll_Number, QR_code INTO #StudentTempTable
                    FROM [dbo].[tbl_StudentMaster]
                    INNER JOIN tbl_CourseClass ON tbl_StudentMaster.class_id = tbl_CourseClass.CourseClass_id
                    INNER JOIN tbl_CourseClassSection ON tbl_StudentMaster.section_id = tbl_CourseClassSection.CourseClassSection_id
                    WHERE tbl_StudentMaster.class_id = @classId AND tbl_StudentMaster.section_id = @sectionId;
                    
                    SELECT * FROM #StudentTempTable
                    ORDER BY student_id
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY;
                    
                    SELECT COUNT(1) FROM #StudentTempTable;
                    
                    DROP TABLE IF EXISTS #StudentTempTable;";

                // Execute the query with pagination
                using (var multi = await _connection.QueryMultipleAsync(sql, new { classId, sectionId, Offset = offset, PageSize = actualPageSize }))
                {
                    var studentList = multi.Read<StudentQRDTO>().ToList();
                    int? totalRecords = (pageSize.HasValue && pageNumber.HasValue) == true ? multi.ReadSingle<int>() : null;

                    if (studentList.Any())
                    {
                        return new ServiceResponse<List<StudentQRDTO>>(true, "Operation successful", studentList, 200, totalRecords);
                    }
                    else
                    {
                        return new ServiceResponse<List<StudentQRDTO>>(false, "Student not found", null, 404);
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
