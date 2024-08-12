using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Repository.Interfaces;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Services.Implementations
{
    public class FunctionalityService : IFunctionalityService
    {
        private readonly IFunctionalityRepository _functionalityRepository;

        public FunctionalityService(IFunctionalityRepository functionalityRepository)
        {
            _functionalityRepository = functionalityRepository;
        }

        public async Task<ServiceResponse<bool>> UpdateFunctionality(UpdateFunctionalityRequest request)
        {
            return await _functionalityRepository.UpdateFunctionality(request);
        }

        public async Task<ServiceResponse<bool>> UpdateFunctionalityStatus(int functionalityId)
        {
            return await _functionalityRepository.UpdateFunctionalityStatus(functionalityId);
        }
    }
}
