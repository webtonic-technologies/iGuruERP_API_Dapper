using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Response;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Repository.Interfaces;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Services.Implementations
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository _moduleRepository;

        public ModuleService(IModuleRepository moduleRepository)
        {
            _moduleRepository = moduleRepository;
        }

        //public async Task<ServiceResponse<List<ModuleResponse>>> GetAllModules()
        //{
        //    return await _moduleRepository.GetAllModules();
        //}

        public async Task<ServiceResponse<List<ModuleResponse>>> GetAllModules(int pageNumber, int pageSize)
        {
            return await _moduleRepository.GetAllModules(pageNumber, pageSize);
        }


        public async Task<ServiceResponse<bool>> UpdateModule(UpdateModuleRequest request)
        {
            return await _moduleRepository.UpdateModule(request);
        }

        public async Task<ServiceResponse<bool>> UpdateModuleStatus(int moduleId)
        {
            return await _moduleRepository.UpdateModuleStatus(moduleId);
        }
    }
}
