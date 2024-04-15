using Dapper;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;
using Institute_API.Repository.Interfaces;
using System.Data;

namespace Institute_API.Repository.Implementations
{
    public class AdminDepartmentRepository : IAdminDepartmentRepository
    {
        private readonly IDbConnection _connection;

        public AdminDepartmentRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<ServiceResponse<string>> AddUpdateAdminDept(AdminDepartment request)
        {
            try
            {
                if (request.Department_id == 0)
                {
                    string sql = @"INSERT INTO [dbo].[tbl_Department] (Institute_id, DepartmentName)
                       VALUES (@Institute_id, @DepartmentName);
                       SELECT SCOPE_IDENTITY();";

                    int insertedId = await _connection.ExecuteScalarAsync<int>(sql, request);
                    if (insertedId > 0)
                    {

                        return new ServiceResponse<string>(true, "Operation successful", "Department added successfully", 200);
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "operation failed", string.Empty, 500);
                    }
                }
                else
                {
                    string sql = @"UPDATE [dbo].[tbl_Department]
                       SET Institute_id = @Institute_id,
                           DepartmentName = @DepartmentName
                       WHERE Department_id = @DepartmentId";

                    // Execute the query and retrieve the number of affected rows
                    int affectedRows = await _connection.ExecuteAsync(sql, new
                    {
                        InstituteId = request.Institute_id,
                        request.DepartmentName,
                        DepartmentId = request.Department_id
                    });
                    if (affectedRows > 0)
                    {
                        return new ServiceResponse<string>(true, "Operation successful", "Department updated successfully", 200);
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "operation failed", string.Empty, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteAdminDepartment(int Department_id)
        {
            try
            {
                string sql = @"DELETE FROM [dbo].[tbl_Department]
                       WHERE Department_id = @DepartmentId";

                // Execute the query and retrieve the number of affected rows
                int affectedRows = await _connection.ExecuteAsync(sql, new { Department_id });
                if (affectedRows > 0)
                {
                    return new ServiceResponse<string>(true, "Operation successful", "Department deleted successfully", 200);
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

        public async Task<ServiceResponse<AdminDepartment>> GetAdminDepartmentById(int Department_id)
        {
            try
            {
                string sql = @"SELECT *
                       FROM [dbo].[tbl_Department]
                       WHERE Department_id = @Department_id";

                // Execute the query and retrieve the department
                var department = await _connection.QueryFirstOrDefaultAsync<AdminDepartment>(sql, new { Department_id });
                if (department != null)
                {
                    return new ServiceResponse<AdminDepartment>(true, "Record found", department, 200);
                }
                else
                {
                    return new ServiceResponse<AdminDepartment>(false, "record not found", new AdminDepartment(), 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AdminDepartment>(false, ex.Message, new AdminDepartment(), 500);
            }
        }

        public async Task<ServiceResponse<List<AdminDepartment>>> GetAdminDepartmentList(int Institute_id)
        {
            try
            {
                string sql = @"SELECT *
                       FROM [dbo].[tbl_Department]
                       WHERE Institute_id = @Institute_id";

                // Execute the query and retrieve the departments
                var departments = await _connection.QueryAsync<AdminDepartment>(sql, new { Institute_id });
                if (departments != null)
                {
                    return new ServiceResponse<List<AdminDepartment>>(true, "Record found", departments.AsList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<AdminDepartment>>(false, "record not found", [], 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AdminDepartment>>(false, ex.Message, [], 500);
            }
        }
    }
}
