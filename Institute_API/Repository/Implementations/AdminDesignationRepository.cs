using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;
using Institute_API.Repository.Interfaces;
using System.Data;
using Dapper;
using Institute_API.DTOs;

namespace Institute_API.Repository.Implementations
{
    public class AdminDesignationRepository : IAdminDesignationRepository
    {
        private readonly IDbConnection _connection;

        public AdminDesignationRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<ServiceResponse<string>> AddUpdateAdminDesignation(AdminDesignation request)
        {
            try
            {
                if (request.Designation_id == 0)
                {
                    string sql = @"INSERT INTO [dbo].[tbl_Designation] (Institute_id, DesignationName, Department_id, IsDeleted)
                       VALUES (@Institute_id, @DesignationName, @Department_id, @IsDeleted);
                       SELECT SCOPE_IDENTITY();"; // Retrieve the inserted id
                    request.IsDeleted = false;
                    // Execute the query and retrieve the inserted id
                    int insertedId = await _connection.ExecuteScalarAsync<int>(sql, request);
                    if (insertedId > 0)
                    {
                        string insertQuery = @"INSERT INTO [dbo].[tbl_AdminConfiguration] (Institute_id, Department_id, Designation_id)
                       VALUES (@Institute_id, @Department_id, @Designation_id);";
                        AdminConfiguration adminConfiguration = new()
                        {
                            Institute_id = request.Institute_id,
                            Department_id = request.Department_id,
                            Designation_id = insertedId
                        };
                        // Execute the query and retrieve the inserted id
                        int rowsAffected = await _connection.ExecuteAsync(insertQuery, adminConfiguration);
                        if (rowsAffected > 0)
                        {
                            return new ServiceResponse<string>(true, "Operation successful", "Designation Added successfully", 200);
                        }
                        else
                        {
                            return new ServiceResponse<string>(false, "operation failed", string.Empty, 500);
                        }
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Some error occured", string.Empty, 500);
                    }
                }
                else
                {
                    string sql = @"UPDATE [dbo].[tbl_Designation]
                       SET Institute_id = @Institute_id,
                           DesignationName = @DesignationName,
                           Department_id = @Department_id,
                           IsDeleted = @IsDeleted,
                       WHERE Designation_id = @Designation_id";
                    request.IsDeleted = false;
                    // Execute the query and retrieve the number of affected rows
                    int affectedRows = await _connection.ExecuteAsync(sql, request);
                    if (affectedRows > 0)
                    {
                        var sqlSelectById = @"
                        SELECT Admin_Configuration_id, Institute_id, Department_id, Designation_id
                        FROM tbl_AdminConfiguration
                        WHERE Designation_id = @Designation_id;";

                        var parameters = new
                        {
                            request.Designation_id
                        };

                        var adminConfiguration = await _connection.QuerySingleOrDefaultAsync<AdminConfiguration>(sqlSelectById, parameters);

                        var sqlUpdate = @"
                        UPDATE tbl_AdminConfiguration
                        SET Institute_id = @Institute_id,
                            Department_id = @Department_id,
                            Designation_id = @Designation_id
                        WHERE Admin_Configuration_id = @Admin_Configuration_id;";
                        AdminConfiguration adminConfiguration1 = new()
                        {
                            Institute_id = request.Institute_id,
                            Department_id = request.Department_id,
                            Designation_id = request.Department_id,
                            Admin_Configuration_id = adminConfiguration != null ? adminConfiguration.Admin_Configuration_id : 0
                        };
                        // Execute the query and retrieve the inserted id
                        int rowsAffected = await _connection.ExecuteAsync(sqlUpdate, adminConfiguration1);
                        if (rowsAffected > 0)
                        {
                            return new ServiceResponse<string>(true, "operation successful", "Designation updated successfully", 200);
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

        public async Task<ServiceResponse<string>> DeleteAdminDesignation(int designationId)
        {
            try
            {
                // Check if the designation is mapped to any employees
                string checkMappingSql = @"SELECT COUNT(*) FROM [iGuruERP].[dbo].[tbl_EmployeeProfileMaster] WHERE Designation_id = @DesignationId AND Status = 1"; // Assuming Status = 1 means active
                int employeeCount = await _connection.ExecuteScalarAsync<int>(checkMappingSql, new { DesignationId = designationId });

                if (employeeCount > 0)
                {
                    return new ServiceResponse<string>(false, "Cannot delete the designation; it is mapped to active employees.", string.Empty, 400);
                }

                // Proceed with the soft delete
                string sql = "UPDATE tbl_Designation SET IsDeleted = @IsDeleted WHERE Designation_id = @DesignationId";

                int affectedRows = await _connection.ExecuteAsync(sql, new { IsDeleted = true, DesignationId = designationId });
                if (affectedRows > 0)
                {
                    return new ServiceResponse<string>(true, "Operation successful", "Designation deleted successfully", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Operation failed", string.Empty, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<AdminDesignation>> GetAdminDesignationById(int Designationid)
        {
            try
            {
                string sql = @"SELECT *
                       FROM [dbo].[tbl_Designation]
                       WHERE Designation_id = @Designation_id AND IsDeleted = 0";

                // Execute the query and retrieve the department
                var designation = await _connection.QueryFirstOrDefaultAsync<AdminDesignation>(sql, new { Designation_id = Designationid });
                if (designation != null)
                {
                    return new ServiceResponse<AdminDesignation>(true, "Record found", designation, 200);
                }
                else
                {
                    return new ServiceResponse<AdminDesignation>(false, "record not found", new AdminDesignation(), 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AdminDesignation>(false, ex.Message, new AdminDesignation(), 500);
            }
        }

        public async Task<ServiceResponse<List<AdminDesignation>>> GetAdminDesignationList(GetListRequest request)
        {
            try
            {
                string sql = @"SELECT *
                       FROM [dbo].[tbl_Designation]
                       WHERE Institute_id = @Institute_id AND IsDeleted = 0";

                // Add search text filter if provided
                if (!string.IsNullOrEmpty(request.SearchText))
                {
                    sql += " AND (DesignationName LIKE @SearchText OR Description LIKE @SearchText)";
                }

                // Add sorting
                string sortColumn = "DesignationName"; // Default sort column
                string sortOrder = request.SortDirection.ToUpper() == "DESC" ? "DESC" : "ASC"; // Validate sort direction

                sql += $" ORDER BY {sortColumn} {sortOrder}";

                // Execute the query and retrieve the designations
                var designations = await _connection.QueryAsync<AdminDesignation>(sql, new
                {
                    Institute_id = request.Institute_id,
                    SearchText = "%" + request.SearchText + "%"
                });

                if (designations != null && designations.Any())
                {
                    // Apply pagination
                    var paginatedDesignations = designations
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToList();

                    return new ServiceResponse<List<AdminDesignation>>(true, "Records found", paginatedDesignations, 200, designations.Count());
                }
                else
                {
                    return new ServiceResponse<List<AdminDesignation>>(false, "Records not found", new List<AdminDesignation>(), 204);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AdminDesignation>>(false, ex.Message, new List<AdminDesignation>(), 500);
            }
        }
    }
}
