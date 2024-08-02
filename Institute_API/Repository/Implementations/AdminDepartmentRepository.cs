using Dapper;
using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;
using Institute_API.Repository.Interfaces;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                    string sql = @"INSERT INTO [dbo].[tbl_Department] (Institute_id, DepartmentName, IsDeleted)
                       VALUES (@Institute_id, @DepartmentName, @IsDeleted);
                       SELECT SCOPE_IDENTITY();";
                    request.IsDeleted = false;
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
                           DepartmentName = @DepartmentName,
                           IsDeleted = @IsDeleted
                       WHERE Department_id = @Department_id";

                    // Execute the query and retrieve the number of affected rows
                    int affectedRows = await _connection.ExecuteAsync(sql, new
                    {
                        request.Institute_id,
                        request.DepartmentName,
                        request.Department_id,
                        IsDeleted = false
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
                string sql = "UPDATE tbl_Department SET IsDeleted = @IsDeleted WHERE Department_id = @Department_id";

                // Execute the query and retrieve the number of affected rows
                int affectedRows = await _connection.ExecuteAsync(sql, new { IsDeleted = true, Department_id });
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
                       WHERE Department_id = @Department_id AND IsDeleted = 0";

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
        public async Task<ServiceResponse<List<AdminDepartment>>> GetAdminDepartmentList(GetListRequest request)
        {
            try
            {
                string sql = @"SELECT *
                       FROM [dbo].[tbl_Department]
                       WHERE Institute_id = @Institute_id AND IsDeleted = 0";

                // Add search text filter if provided
                if (!string.IsNullOrEmpty(request.SearchText))
                {
                    sql += " AND (DepartmentName LIKE @SearchText OR Description LIKE @SearchText)";
                }

                // Add sorting
                string sortColumn = "DepartmentName"; // Default sort column
                string sortOrder = request.SortDirection.ToUpper() == "DESC" ? "DESC" : "ASC"; // Validate sort direction

                sql += $" ORDER BY {sortColumn} {sortOrder}";

                // Execute the query and retrieve the departments
                var departments = await _connection.QueryAsync<AdminDepartment>(sql, new
                {
                    Institute_id = request.Institute_id,
                    SearchText = "%" + request.SearchText + "%"
                });

                if (departments != null && departments.Any())
                {
                    // Apply pagination
                    var paginatedDepartments = departments
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToList();

                    return new ServiceResponse<List<AdminDepartment>>(true, "Records found", paginatedDepartments, 200, departments.Count());
                }
                else
                {
                    return new ServiceResponse<List<AdminDepartment>>(false, "Records not found", new List<AdminDepartment>(), 204);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AdminDepartment>>(false, ex.Message, new List<AdminDepartment>(), 500);
            }
        }
    }
}
