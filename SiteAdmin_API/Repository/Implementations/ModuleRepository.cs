using Dapper;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;
using SiteAdmin_API.Repository.Interfaces;
using System.Data;
using System.Linq;

namespace SiteAdmin_API.Repository.Implementations
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly IDbConnection _connection;

        public ModuleRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<Module>>> GetAllModules(GetAllModulesRequest request)
        {
            try
            {
                string sql = "SELECT * FROM tblModules WHERE IsActive = 1";
                var modules = await _connection.QueryAsync<Module>(sql);
                var paginatedList = modules.Skip((request.PageNumber - 1) * request.PageSize)
                                           .Take(request.PageSize)
                                           .ToList();
                return new ServiceResponse<List<Module>>(true, "Modules retrieved successfully", paginatedList, 200, modules.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Module>>(false, ex.Message, null, 500);
            }
        }

        // Other methods for Modules
    }
}
