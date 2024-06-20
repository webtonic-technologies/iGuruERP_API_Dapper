using Dapper;
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using System.Data;

namespace Student_API.Repository.Implementations
{
    public class StudentPromotionRepository : IStudentPromotionRepository
    {
        private readonly IDbConnection _connection;

        public StudentPromotionRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<StudentPromotionDTO>>> GetStudentsForPromotion(int classId, string sortField, string sortDirection, int? pageSize = null, int? pageNumber = null)
        {
            try
            {
                // Validate sortField and sortDirection
                var allowedSortFields = new List<string> { "Student_Name", "Class_Section" };
                var allowedSortDirections = new List<string> { "ASC", "DESC" };

                // Default to "Student_Name" if sortField is invalid
                if (!allowedSortFields.Contains(sortField))
                {
                    sortField = "Student_Name";
                }

                // Default to "ASC" if sortDirection is invalid
                sortDirection = sortDirection.ToUpper();
                if (!allowedSortDirections.Contains(sortDirection))
                {
                    sortDirection = "ASC";
                }

                // Map sortField to SQL column names
                string sortColumn;
                switch (sortField)
                {
                    case "Class_Section":
                        sortColumn = "Class_Section";
                        break;
                    case "Student_Name":
                    default:
                        sortColumn = "Student_Name";
                        break;
                }

                string query = $@"
        IF OBJECT_ID('tempdb..#TempStudentDetails') IS NOT NULL
    DROP TABLE #TempStudentDetails;

        SELECT 
            student_id, 
            CONCAT(first_name, ' ', last_name) AS Student_Name, 
            CONCAT(tbl_CourseClass.class_course, ' - ', tbl_CourseClassSection.Section) AS Class_Section,
            tbl_StudentMaster.class_id,
            tbl_StudentMaster.Section_Id
        INTO 
            #TempStudentDetails
        FROM 
            tbl_StudentMaster
        INNER JOIN 
            tbl_CourseClass ON tbl_CourseClass.CourseClass_id = tbl_StudentMaster.class_id
        INNER JOIN 
            tbl_CourseClassSection ON tbl_CourseClassSection.CourseClassSection_id = tbl_StudentMaster.section_id
        WHERE 
            tbl_StudentMaster.class_id = @ClassId;

       
        SELECT 
            COUNT(*) 
        FROM 
            #TempStudentDetails;

      
        SELECT 
            student_id, 
            Student_Name, 
            Class_Section,
            class_id,
            Section_Id
        FROM 
            #TempStudentDetails
        ORDER BY 
            {sortColumn} {sortDirection}, student_id
        OFFSET 
            @Offset ROWS
        FETCH NEXT 
            @PageSize ROWS ONLY;

       IF OBJECT_ID('tempdb..#TempStudentDetails') IS NOT NULL
    DROP TABLE #TempStudentDetails;

        ";

                List<StudentPromotionDTO> students;
                int totalRecords = 0;

                if (pageSize.HasValue && pageNumber.HasValue)
                {
                    int offset = (pageNumber.Value - 1) * pageSize.Value;

                    using (var multi = await _connection.QueryMultipleAsync(query, new { ClassId = classId, Offset = offset, PageSize = pageSize }))
                    {
                        totalRecords = multi.ReadSingle<int>();
                        students = multi.Read<StudentPromotionDTO>().ToList();
                    }

                    return new ServiceResponse<List<StudentPromotionDTO>>(true, "Students retrieved successfully", students, 200, totalRecords);
                }
                else
                {
                    using (var multi = await _connection.QueryMultipleAsync(query, new { ClassId = classId, Offset = 0, PageSize = int.MaxValue }))
                    {
                        totalRecords = multi.ReadSingle<int>();
                        students = multi.Read<StudentPromotionDTO>().ToList();
                    }

                    return new ServiceResponse<List<StudentPromotionDTO>>(true, "All students retrieved successfully", students, 200, totalRecords);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentPromotionDTO>>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<bool>> PromoteStudents(List<int> studentIds, int nextClassId)
        {
            try
            {
                string query = @"
                UPDATE tbl_StudentMaster
                SET class_id = @NextClassId
                WHERE student_id IN @StudentIds";

                await _connection.ExecuteAsync(query, new { NextClassId = nextClassId, StudentIds = studentIds });

                return new ServiceResponse<bool>(true, "Students promoted successfully", true, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}