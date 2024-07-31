using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;
using SiteAdmin_API.Repository.Interfaces;
using SiteAdmin_API.Services.Interfaces;

namespace SiteAdmin_API.Services.Implementations
{
    public class CreatePackageService : ICreatePackageService
    {
        private readonly ICreatePackageRepository _createPackageRepository;

        public CreatePackageService(ICreatePackageRepository createPackageRepository)
        {
            _createPackageRepository = createPackageRepository;
        }

        public async Task<ServiceResponse<Package>> CreatePackage(CreatePackageRequest request)
        {
            return await _createPackageRepository.CreatePackage(request);
        }
    }
}
