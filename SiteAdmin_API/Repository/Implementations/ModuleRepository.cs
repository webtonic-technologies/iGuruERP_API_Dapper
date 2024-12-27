using Dapper;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Response;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;
using SiteAdmin_API.Repository.Interfaces;
using System.Data;

namespace SiteAdmin_API.Repository.Implementations
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly IDbConnection _connection;

        public ModuleRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        //public async Task<ServiceResponse<List<ModuleResponse>>> GetAllModules()
        //{
        //    try
        //    {
        //        string sql = @"SELECT ModuleID, ModuleName, Description, Icon, ModuleOrder, IsActive FROM tblModules";
        //        var modules = await _connection.QueryAsync<ModuleResponse>(sql);

        //        return new ServiceResponse<List<ModuleResponse>>(true, "Modules retrieved successfully", modules.ToList(), 200);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<List<ModuleResponse>>(false, ex.Message, null, 500);
        //    }
        //}

        public async Task<ServiceResponse<List<ModuleResponse>>> GetAllModules(int pageNumber, int pageSize)
        {
            try
            {
                // Calculate the OFFSET based on the page number and page size
                int offset = (pageNumber - 1) * pageSize;

                string sql = @"
            SELECT ModuleID, ModuleName, Description, Icon, ModuleOrder, IsActive 
            FROM tblModules
            ORDER BY ModuleID 
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                // Execute the query with parameters for OFFSET and PageSize
                var modules = await _connection.QueryAsync<ModuleResponse>(sql, new { Offset = offset, PageSize = pageSize });

                // Get the total count of modules for pagination metadata
                string countSql = "SELECT COUNT(*) FROM tblModules";
                var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

                return new ServiceResponse<List<ModuleResponse>>(true, "Modules retrieved successfully", modules.ToList(), 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ModuleResponse>>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<bool>> UpdateModule(UpdateModuleRequest request)
        {
            try
            {
                string updateSql = @"
                    UPDATE tblModules
                    SET ModuleName = @ModuleName,
                        Description = @Description,
                        Icon = @Icon,
                        ModuleOrder = @ModuleOrder,
                        IsActive = @IsActive
                    WHERE ModuleID = @ModuleID";

                var rowsAffected = await _connection.ExecuteAsync(updateSql, request);

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Module updated successfully", true, 200);
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Module update failed", false, 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateModuleStatus(int moduleId)
        {
            try
            {
                // Toggle the IsActive status
                string query = "UPDATE tblModules SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE ModuleID = @ModuleID";
                int rowsAffected = await _connection.ExecuteAsync(query, new { ModuleID = moduleId });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Module status updated successfully", true, 200);
                }

                return new ServiceResponse<bool>(false, "Failed to update module status", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
