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

        public async Task<ServiceResponse<List<StudentPromotionDTO>>> GetStudentsForPromotion(int classId, int? pageSize = null, int? pageNumber = null)
        {
            try
            {
                string queryAll = @"
                SELECT student_id, first_name, last_name, class_id
                FROM tbl_StudentMaster
                WHERE class_id = @ClassId";

                string queryCount = @"
                SELECT COUNT(*)
                FROM tbl_StudentMaster
                WHERE class_id = @ClassId";

                List<StudentPromotionDTO> students;
                int totalRecords = 0;

                if (pageSize.HasValue && pageNumber.HasValue)
                {
                    int offset = (pageNumber.Value - 1) * pageSize.Value;
                    string queryPaginated = $@"
                    {queryAll}
                    ORDER BY student_id
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY;

                    {queryCount}";

                    using (var multi = await _connection.QueryMultipleAsync(queryPaginated, new { ClassId = classId, Offset = offset, PageSize = pageSize }))
                    {
                        students = multi.Read<StudentPromotionDTO>().ToList();
                        totalRecords = multi.ReadSingle<int>();
                    }

                    return new ServiceResponse<List<StudentPromotionDTO>>(true, "Students retrieved successfully", students, 200, totalRecords);
                }
                else
                {
                    students = (await _connection.QueryAsync<StudentPromotionDTO>(queryAll, new { ClassId = classId })).ToList();
                    return new ServiceResponse<List<StudentPromotionDTO>>(true, "All students retrieved successfully", students, 200);
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