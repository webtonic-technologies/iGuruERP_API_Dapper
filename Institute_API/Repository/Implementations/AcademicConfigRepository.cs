using Dapper;
using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;
using Institute_API.Repository.Interfaces;
using System.Data;

namespace Institute_API.Repository.Implementations
{
    public class AcademicConfigRepository : IAcademicConfigRepository
    {
        private readonly IDbConnection _connection;

        public AcademicConfigRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<ServiceResponse<string>> AddUpdateAcademicConfig(CourseClassDTO request)
        {
            try
            {
                if (request.CourseClass_id == 0)
                {
                    string sql = @"INSERT INTO [dbo].[tbl_CourseClass] (Institute_id, class_course)
                       VALUES (@InstituteId, @ClassCourse);
                       SELECT SCOPE_IDENTITY();"; // Retrieve the inserted id

                    // Execute the query and retrieve the inserted id
                    int insertedId = await _connection.ExecuteScalarAsync<int>(sql, new
                    {
                        InstituteId = request.Institute_id,
                        ClassCourse = request.Class_course
                    });
                    if (insertedId > 0 || request.CourseClassSections != null)
                    {
                        int section = await AddUpdateCourseClassSection(request.CourseClassSections ??= ([]), insertedId);
                        if (section > 0)
                        {
                            return new ServiceResponse<string>(true, "Operation successful", "Data added successfully", 500);
                        }
                        else
                        {
                            return new ServiceResponse<string>(false, "Some error occured", string.Empty, 500);
                        }
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Some error occured", string.Empty, 500);
                    }
                }
                else
                {
                    string sql = @"UPDATE [dbo].[tbl_CourseClass]
                       SET Institute_id = @InstituteId,
                           class_course = @ClassCourse
                       WHERE CourseClass_id = @CourseClassId";

                    // Execute the query and retrieve the number of affected rows
                    int affectedRows = await _connection.ExecuteAsync(sql, new
                    {
                        InstituteId = request.Institute_id,
                        ClassCourse = request.Class_course,
                        CourseClassId = request.CourseClass_id
                    });
                    if (affectedRows > 0 || request.CourseClassSections != null)
                    {
                        int section = await AddUpdateCourseClassSection(request.CourseClassSections ??= ([]), request.CourseClass_id);
                        if (section > 0)
                        {
                            return new ServiceResponse<string>(true, "Operation successful", "Data added successfully", 500);
                        }
                        else
                        {
                            return new ServiceResponse<string>(false, "Some error occured", string.Empty, 500);
                        }
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Some error occured", string.Empty, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteAcademicConfig(int CourseClass_id)
        {
            try
            {
                string sql = @"DELETE FROM [dbo].[tbl_CourseClass]
                       WHERE CourseClass_id = @CourseClass_id";

                // Execute the query and retrieve the number of affected rows
                int affectedRows = await _connection.ExecuteAsync(sql, new { CourseClass_id });
                if (affectedRows > 0)
                {
                    string deleteQuery = "DELETE FROM tbl_CourseClassSection WHERE CourseClass_id = @CourseClass_id";
                    int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { CourseClass_id });

                    return new ServiceResponse<string>(true, "Operation successful", "Data deleted successfully", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "operation failed", string.Empty, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<CourseClassDTO>> GetAcademicConfigById(int CourseClass_id)
        {
            try
            {
                var response = new CourseClassDTO();
                string sql = @"SELECT *
                       FROM [dbo].[tbl_CourseClass]
                       WHERE CourseClass_id = @CourseClass_id";

                // Execute the query and retrieve the department
                var data = await _connection.QueryFirstOrDefaultAsync<CourseClass>(sql, new { CourseClass_id });
                if (data != null)
                {
                    response.CourseClass_id = data.CourseClass_id;
                    response.Class_course = data.Class_course;
                    response.Institute_id = data.Institute_id;
                    string query = "SELECT * FROM tbl_CourseClassSection WHERE CourseClass_id = @CourseClass_id";
                    var sections = await _connection.QueryAsync<CourseClassSection>(query, new { CourseClass_id });
                    if (sections != null)
                    {
                        response.CourseClassSections = sections.AsList();
                    }
                    return new ServiceResponse<CourseClassDTO>(true, "Record found", response, 200);
                }
                else
                {
                    return new ServiceResponse<CourseClassDTO>(false, "record not found", new CourseClassDTO(), 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<CourseClassDTO>(false, ex.Message, new CourseClassDTO(), 500);
            }
        }

        public async Task<ServiceResponse<List<CourseClassDTO>>> GetAcademicConfigList(int Institute_id)
        {
            try
            {
                var response = new List<CourseClassDTO>();
                string sql = @"SELECT *
                       FROM [dbo].[tbl_CourseClass]
                       WHERE Institute_id = @Institute_id";

                // Execute the query and retrieve the department
                var data = await _connection.QueryAsync<CourseClass>(sql, new { Institute_id });
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var record = new CourseClassDTO
                        {
                            Institute_id = item.Institute_id,
                            Class_course = item.Class_course,
                            CourseClass_id = item.CourseClass_id
                        };

                        string query = "SELECT * FROM tbl_CourseClassSection WHERE CourseClass_id = @CourseClass_id";
                        var sections = await _connection.QueryAsync<CourseClassSection>(query, new { item.CourseClass_id });
                        if (sections != null)
                        {
                            record.CourseClassSections = sections.AsList();
                        }
                        response.Add(record);
                    }
                    return new ServiceResponse<List<CourseClassDTO>>(true, "Record found", response, 200);
                }
                else
                {
                    return new ServiceResponse<List<CourseClassDTO>>(false, "record not found", [], 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<CourseClassDTO>>(false, ex.Message, [], 500);
            }
        }
        private async Task<int> AddUpdateCourseClassSection(List<CourseClassSection> request, int CourseClass_id)
        {
            int addedRecords = 0;
            if (request != null)
            {
                foreach (var data in request)
                {
                    data.CourseClass_id = CourseClass_id;
                }
            }
            string query = "SELECT COUNT(*) FROM tbl_CourseClassSection WHERE CourseClass_id = @CourseClass_id";
            int count = await _connection.ExecuteScalarAsync<int>(query, new { CourseClass_id });
            if (count > 0)
            {
                string deleteQuery = "DELETE FROM tbl_CourseClassSection WHERE CourseClass_id = @CourseClass_id";
                int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { CourseClass_id });
                if (rowsAffected > 0)
                {
                    string insertQuery = @"INSERT INTO tbl_CourseClassSection (CourseClass_id, Section)
                       VALUES (@CourseClass_id, @Section);";

                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                }
            }
            else
            {
                string insertQuery = @"INSERT INTO tbl_CourseClassSection (CourseClass_id, Section)
                       VALUES (@CourseClass_id, @Section);";
                // Execute the query with multiple parameterized sets of values
                addedRecords = await _connection.ExecuteAsync(insertQuery, request);
            }
            return addedRecords;
        }
    }
}
