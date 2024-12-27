using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;
using SiteAdmin_API.Repository.Interfaces;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Services.Implementations
{
    public class InstituteOnboardService : IInstituteOnboardService
    {
        private readonly IInstituteOnboardRepository _instituteOnboardRepository;

        public InstituteOnboardService(IInstituteOnboardRepository instituteOnboardRepository)
        {
            _instituteOnboardRepository = instituteOnboardRepository;
        }

        public async Task<ServiceResponse<InstituteOnboard>> AddUpdateInstituteOnboard(InstituteOnboardRequest request)
        {
            return await _instituteOnboardRepository.AddUpdateInstituteOnboard(request);
        }

        public async Task<ServiceResponse<List<InstituteOnboard>>> GetAllInstituteOnboard(int pageNumber, int pageSize)
        {
            return await _instituteOnboardRepository.GetAllInstituteOnboard(pageNumber, pageSize);
        }

        public async Task<ServiceResponse<InstituteOnboard>> GetInstituteOnboardById(int instituteOnboardId)
        {
            return await _instituteOnboardRepository.GetInstituteOnboardById(instituteOnboardId);
        }
    }
}
