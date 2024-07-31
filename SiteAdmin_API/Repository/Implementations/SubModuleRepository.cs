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


    }
}
