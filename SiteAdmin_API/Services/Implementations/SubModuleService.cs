using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Response;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;
using SiteAdmin_API.Repository.Interfaces;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Services.Implementations
{
    public class SubModuleService : ISubModuleService
    {
        private readonly ISubModuleRepository _subModuleRepository;

        public SubModuleService(ISubModuleRepository subModuleRepository)
        {
            _subModuleRepository = subModuleRepository;
        }

        public async Task<ServiceResponse<List<SubModule>>> GetAllSubModules(GetAllSubModulesRequest request)
        {
            return await _subModuleRepository.GetAllSubModules(request);
        }

        public async Task<ServiceResponse<List<FunctionalityResponse>>> GetAllFunctionality(GetAllFunctionalityRequest request)
        {
            return await _subModuleRepository.GetAllFunctionality(request);
        }
    }
}
