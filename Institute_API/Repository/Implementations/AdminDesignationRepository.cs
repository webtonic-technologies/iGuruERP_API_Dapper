using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;
using Institute_API.Repository.Interfaces;
using System.Data;
using Dapper;

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
                    string sql = @"INSERT INTO [dbo].[tbl_Designation] (Institute_id, DesignationName, Department_id)
                       VALUES (@InstituteId, @DesignationName, @DepartmentId);
                       SELECT SCOPE_IDENTITY();"; // Retrieve the inserted id

                    // Execute the query and retrieve the inserted id
                    int insertedId = await _connection.ExecuteScalarAsync<int>(sql, request);
                    if (insertedId > 0)
                    {
                        string insertQuery = @"INSERT INTO [dbo].[tbl_AdminConfiguration] (Institute_id, Department_id, Designation_id)
                       VALUES (@InstituteId, @DepartmentId, @DesignationId);";
                        AdminConfiguration adminConfiguration = new()
                        {
                            Institute_id = request.Institute_id,
                            Department_id = request.Department_id,
                            Designation_id = insertedId
                        };
                        // Execute the query and retrieve the inserted id
                        int rowsAffected = await _connection.ExecuteScalarAsync<int>(insertQuery, adminConfiguration);
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
                       SET Institute_id = @InstituteId,
                           DesignationName = @DesignationName,
                           Department_id = @DepartmentId
                       WHERE Designation_id = @DesignationId";

                    // Execute the query and retrieve the number of affected rows
                    int affectedRows = await _connection.ExecuteAsync(sql, request);
                    if (affectedRows > 0)
                    {
                        return new ServiceResponse<string>(true, "operation successful", "Designation updated successfully", 500);
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

        public async Task<ServiceResponse<string>> DeleteAdminDesignation(int Designation_id)
        {
            try
            {
                string sql = @"DELETE FROM [dbo].[tbl_Designation]
                       WHERE Designation_id = @Designation_id";

                // Execute the query and retrieve the number of affected rows
                int affectedRows = await _connection.ExecuteAsync(sql, new { Designation_id });
                if (affectedRows > 0)
                {
                    return new ServiceResponse<string>(true, "Operation successful", "Designation deleted successfully", 200);
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

        public async Task<ServiceResponse<AdminDesignation>> GetAdminDesignationById(int Designationid)
        {
            try
            {
                string sql = @"SELECT *
                       FROM [dbo].[tbl_Designation]
                       WHERE Designation_id = @Designation_id";

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

        public async Task<ServiceResponse<List<AdminDesignation>>> GetAdminDesignationList(int Institute_id)
        {
            try
            {
                string sql = @"SELECT *
                       FROM [dbo].[tbl_Designation]
                       WHERE Institute_id = @InstituteId";

                // Execute the query and retrieve the departments
                var designations = await _connection.QueryAsync<AdminDesignation>(sql, new { Institute_id });
                if (designations != null)
                {
                    return new ServiceResponse<List<AdminDesignation>>(true, "Record found", designations.AsList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<AdminDesignation>>(false, "record not found", [], 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AdminDesignation>>(false, ex.Message, [], 500);
            }
        }
    }
}
