using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;

namespace SiteAdmin_API.Repository.Interfaces
{
    public interface IModuleRepository
    {
        Task<ServiceResponse<List<Module>>> GetAllModules(GetAllModulesRequest request);
        // Other methods for Modules
    }
}
