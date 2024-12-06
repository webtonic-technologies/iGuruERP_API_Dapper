using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;

namespace SiteAdmin_API.Repository.Interfaces
{
    public interface IPackageRepository
    {
        Task<ServiceResponse<Package>> AddUpdatePackage(AddUpdatePackageRequest request);
        Task<ServiceResponse<List<GetPackage>>> GetAllPackages();
        Task<ServiceResponse<bool>> UpdatePackageStatus(int packageId);
    }
}
