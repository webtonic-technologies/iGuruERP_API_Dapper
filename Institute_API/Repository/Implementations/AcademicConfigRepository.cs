using Dapper;
using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
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
        public async Task<ServiceResponse<string>> AddUpdateAcademicConfig(Class request)
        {
            try
            {
                if (request.class_id == 0)
                {
                    string insertClassSql = @"INSERT INTO [dbo].[tbl_Class] (class_name, institute_id, IsDeleted)
                                      VALUES (@ClassName, @InstituteId, @IsDeleted);
                                      SELECT SCOPE_IDENTITY();"; // Retrieve the inserted id

                    // Execute the query and retrieve the inserted id
                    int insertedClassId = await _connection.ExecuteScalarAsync<int>(insertClassSql, new
                    {
                        ClassName = request.class_name,
                        InstituteId = request.institute_id,
                        IsDeleted = false
                    });

                    if (insertedClassId > 0)
                    {
                        int sectionResult = await AddUpdateSections(request.Sections ?? new List<Section>(), insertedClassId);
                        if (sectionResult > 0)
                        {
                            return new ServiceResponse<string>(true, "Operation successful", "Data added successfully", 200);
                        }
                        else
                        {
                            return new ServiceResponse<string>(false, "Error occurred while adding sections", string.Empty, 500);
                        }
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Error occurred while adding class", string.Empty, 500);
                    }
                }
                else
                {
                    string updateClassSql = @"UPDATE [dbo].[tbl_Class]
                                      SET class_name = @ClassName,
                                          institute_id = @InstituteId,
                                          IsDeleted = @IsDeleted
                                      WHERE class_id = @ClassId";

                    // Execute the query and retrieve the number of affected rows
                    int affectedRows = await _connection.ExecuteAsync(updateClassSql, new
                    {
                        ClassName = request.class_name,
                        InstituteId = request.institute_id,
                        ClassId = request.class_id,
                        IsDeleted = false
                    });

                    if (affectedRows > 0)
                    {
                        int sectionResult = await AddUpdateSections(request.Sections ?? new List<Section>(), request.class_id);
                        if (sectionResult > 0)
                        {
                            return new ServiceResponse<string>(true, "Operation successful", "Data updated successfully", 200);
                        }
                        else
                        {
                            return new ServiceResponse<string>(false, "Error occurred while updating sections", string.Empty, 500);
                        }
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Error occurred while updating class", string.Empty, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }
        public async Task<ServiceResponse<string>> DeleteAcademicConfig(int classId)
        {
            try
            {
                // Soft delete the class by setting IsDeleted to true
                string updateClassSql = @"UPDATE [dbo].[tbl_Class]
                                  SET IsDeleted = 1
                                  WHERE class_id = @ClassId";

                // Execute the query and retrieve the number of affected rows
                int affectedRows = await _connection.ExecuteAsync(updateClassSql, new { ClassId = classId });

                if (affectedRows > 0)
                {
                    // Soft delete the sections related to this class
                    string updateSectionSql = @"UPDATE [dbo].[tbl_Section]
                                        SET IsDeleted = 1
                                        WHERE class_id = @ClassId";

                    int sectionAffectedRows = await _connection.ExecuteAsync(updateSectionSql, new { ClassId = classId });

                    return new ServiceResponse<string>(true, "Operation successful", "Data deleted successfully", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Operation failed", string.Empty, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }
        public async Task<ServiceResponse<Class>> GetAcademicConfigById(int classId)
        {
            try
            {
                var response = new Class();
                string sql = @"SELECT *
                       FROM [dbo].[tbl_Class]
                       WHERE class_id = @ClassId AND IsDeleted = 0";

                // Execute the query and retrieve the class
                var data = await _connection.QueryFirstOrDefaultAsync<Class>(sql, new { ClassId = classId });
                if (data != null)
                {
                    response.class_id = data.class_id;
                    response.class_name = data.class_name;
                    response.institute_id = data.institute_id;
                    response.IsDeleted = data.IsDeleted;

                    string query = "SELECT * FROM tbl_Section WHERE class_id = @ClassId AND IsDeleted = 0";
                    var sections = await _connection.QueryAsync<Section>(query, new { ClassId = classId });
                    if (sections != null)
                    {
                        response.Sections = sections.AsList();
                    }
                    return new ServiceResponse<Class>(true, "Record found", response, 200);
                }
                else
                {
                    return new ServiceResponse<Class>(false, "Record not found", new Class(), 204);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Class>(false, ex.Message, new Class(), 500);
            }
        }
        public async Task<ServiceResponse<List<Class>>> GetAcademicConfigList(GetAllCourseClassRequest request)
        {
            try
            {
                var response = new List<Class>();
                string sql = @"SELECT *
                       FROM [dbo].[tbl_Class]
                       WHERE Institute_id = @Institute_id AND IsDeleted = 0";

                // Add search filter if SearchText is provided
                if (!string.IsNullOrEmpty(request.SearchText))
                {
                    sql += " AND class_name LIKE @SearchText";
                }

                // Execute the query and retrieve the classes
                var data = await _connection.QueryAsync<Class>(sql, new { request.Institute_id, SearchText = $"%{request.SearchText}%" });
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var record = new Class
                        {
                            class_id = item.class_id,
                            class_name = item.class_name,
                            institute_id = item.institute_id,
                            IsDeleted = item.IsDeleted
                        };

                        string query = "SELECT * FROM tbl_Section WHERE class_id = @ClassId AND IsDeleted = 0";
                        var sections = await _connection.QueryAsync<Section>(query, new { ClassId = item.class_id });
                        if (sections != null)
                        {
                            record.Sections = sections.AsList();
                        }
                        response.Add(record);
                    }

                    // Pagination
                    var paginatedList = response.Skip((request.PageNumber - 1) * request.PageSize)
                                                .Take(request.PageSize)
                                                .ToList();
                    if (paginatedList.Count != 0)
                    {
                        return new ServiceResponse<List<Class>>(true, "Record found", paginatedList, 200);
                    }
                    else
                    {
                        return new ServiceResponse<List<Class>>(false, "Record not found", new List<Class>(), 204);
                    }
                }
                else
                {
                    return new ServiceResponse<List<Class>>(false, "Record not found", new List<Class>(), 204);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Class>>(false, ex.Message, new List<Class>(), 500);
            }
        }
        private async Task<int> AddUpdateSections(List<Section> sections, int classId)
        {
            int result = 0;
            foreach (var section in sections)
            {
                if (section.section_id == 0)
                {
                    string insertSectionSql = @"INSERT INTO [dbo].[tbl_Section] (section_name, class_id, IsDeleted)
                                        VALUES (@SectionName, @ClassId, @IsDeleted)";
                    result += await _connection.ExecuteAsync(insertSectionSql, new
                    {
                        SectionName = section.section_name,
                        ClassId = classId,
                        IsDeleted = false
                    });
                }
                else
                {
                    string updateSectionSql = @"UPDATE [dbo].[tbl_Section]
                                        SET section_name = @SectionName,
                                            class_id = @ClassId,
                                            IsDeleted = @IsDeleted
                                        WHERE section_id = @SectionId";
                    result += await _connection.ExecuteAsync(updateSectionSql, new
                    {
                        SectionName = section.section_name,
                        ClassId = classId,
                        SectionId = section.section_id,
                        IsDeleted = false
                    });
                }
            }
            return result;
        }

    }
}
