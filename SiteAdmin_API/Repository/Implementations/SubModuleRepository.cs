using Dapper;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Response;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;
using SiteAdmin_API.Repository.Interfaces;
using System.Data;
using System.Linq;

namespace SiteAdmin_API.Repository.Implementations
{
    public class SubModuleRepository : ISubModuleRepository
    {
        private readonly IDbConnection _connection;

        public SubModuleRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<SubModule>>> GetAllSubModules(GetAllSubModulesRequest request)
        {
            try
            {
                string sql = "SELECT * FROM tblSubModule WHERE ModuleId = @ModuleId AND IsActive = 1";
                var subModules = await _connection.QueryAsync<SubModule>(sql, new { request.ModuleId });
                var paginatedList = subModules.Skip((request.PageNumber - 1) * request.PageSize)
                                              .Take(request.PageSize)
                                              .ToList();
                return new ServiceResponse<List<SubModule>>(true, "SubModules retrieved successfully", paginatedList, 200, subModules.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<SubModule>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<FunctionalityResponse>>> GetAllFunctionality(GetAllFunctionalityRequest request)
        {
            try
            {
                string sql = "SELECT FunctionalityID, SubModuleID, Functionality, IsActive FROM tblFunctionality WHERE SubModuleId = @SubModuleId AND IsActive = 1";
                var functionalities = await _connection.QueryAsync<FunctionalityResponse>(sql, new { request.SubModuleId });
                var paginatedList = functionalities.Skip((request.PageNumber - 1) * request.PageSize)
                                                   .Take(request.PageSize)
                                                   .ToList();
                return new ServiceResponse<List<FunctionalityResponse>>(true, "Functionalities retrieved successfully", paginatedList, 200, functionalities.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<FunctionalityResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateSubModule(UpdateSubModuleRequest request)
        {
            try
            {
                string updateSql = @"
                    UPDATE tblSubModule
                    SET SubModuleName = @SubModuleName,
                        ModuleID = @ModuleID,
                        IsActive = @IsActive
                    WHERE SubModuleID = @SubModuleID";

                int rowsAffected = await _connection.ExecuteAsync(updateSql, request);

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "SubModule updated successfully", true, 200);
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Failed to update SubModule", false, 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateSubModuleStatus(int subModuleId)
        {
            try
            {
                // Toggle the IsActive status
                string query = "UPDATE tblSubModule SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE SubModuleID = @SubModuleID";
                int rowsAffected = await _connection.ExecuteAsync(query, new { SubModuleID = subModuleId });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "SubModule status updated successfully", true, 200);
                }

                return new ServiceResponse<bool>(false, "Failed to update SubModule status", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }


    }
}
