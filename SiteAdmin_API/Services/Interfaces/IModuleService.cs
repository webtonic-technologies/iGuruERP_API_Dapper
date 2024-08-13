using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Response;
using SiteAdmin_API.DTOs.ServiceResponse;

namespace SiteAdmin_API.Services.Interfaces
{
    public interface IModuleService
    {
        Task<ServiceResponse<List<ModuleResponse>>> GetAllModules();
        Task<ServiceResponse<bool>> UpdateModule(UpdateModuleRequest request);

        Task<ServiceResponse<bool>> UpdateModuleStatus(int moduleId);


    }
}
