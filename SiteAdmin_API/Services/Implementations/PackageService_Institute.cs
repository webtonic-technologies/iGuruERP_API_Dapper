using SiteAdmin_API.DTOs.Response;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Repository.Interfaces;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Services.Implementations
{
    public class PackageService_Institute : IPackageService_Institute
    {
        private readonly IPackageRepository_Institute _packageRepository;

        public PackageService_Institute(IPackageRepository_Institute packageRepository)
        {
            _packageRepository = packageRepository;
        }

        public async Task<ServiceResponse<List<PackageResponse_Institute>>> GetAllPackagesForInstitute()
        {
            return await _packageRepository.GetAllPackagesForInstitute();
        }
    }
}
