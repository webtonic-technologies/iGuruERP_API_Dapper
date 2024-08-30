using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;
using SiteAdmin_API.Repository.Interfaces;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Services.Implementations
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;

        public PackageService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        public async Task<ServiceResponse<Package>> AddUpdatePackage(AddUpdatePackageRequest request)
        {
            return await _packageRepository.AddUpdatePackage(request);
        }

        public async Task<ServiceResponse<List<Package>>> GetAllPackages()
        {
            return await _packageRepository.GetAllPackages();
        }

        public async Task<ServiceResponse<bool>> UpdatePackageStatus(int packageId)
        {
            return await _packageRepository.UpdatePackageStatus(packageId);
        }
    }
}
