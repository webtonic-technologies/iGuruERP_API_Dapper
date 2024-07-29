using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;

namespace SiteAdmin_API.Services.Interfaces
{
    public interface IModuleService
    {
        Task<ServiceResponse<List<Module>>> GetAllModules(GetAllModulesRequest request);
        // Other methods for Modules
    }
}
