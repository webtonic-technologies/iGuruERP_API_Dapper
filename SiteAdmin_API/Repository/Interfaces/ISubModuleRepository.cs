using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Response;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;

namespace SiteAdmin_API.Repository.Interfaces
{
    public interface ISubModuleRepository
    {
        Task<ServiceResponse<List<SubModule>>> GetAllSubModules(GetAllSubModulesRequest request);
        Task<ServiceResponse<List<FunctionalityResponse>>> GetAllFunctionality(GetAllFunctionalityRequest request);
        Task<ServiceResponse<bool>> UpdateSubModule(UpdateSubModuleRequest request);
        Task<ServiceResponse<bool>> UpdateSubModuleStatus(int subModuleId);

    }
}
