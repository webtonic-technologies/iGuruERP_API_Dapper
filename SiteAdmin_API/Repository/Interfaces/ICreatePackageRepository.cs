using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;

namespace SiteAdmin_API.Repository.Interfaces
{
    public interface ICreatePackageRepository
    {
        Task<ServiceResponse<Package>> CreatePackage(CreatePackageRequest request);
        Task<ServiceResponse<Package>> UpdatePackage(UpdatePackageRequest request);
        Task<ServiceResponse<bool>> UpdatePackageStatus(int packageId);

    }
}
