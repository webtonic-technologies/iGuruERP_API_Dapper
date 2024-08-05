using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Response;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;

namespace SiteAdmin_API.Services.Interfaces
{
    public interface ISubModuleService
    {
        Task<ServiceResponse<List<SubModule>>> GetAllSubModules(GetAllSubModulesRequest request);
        Task<ServiceResponse<List<FunctionalityResponse>>> GetAllFunctionality(GetAllFunctionalityRequest request);
    }

}
