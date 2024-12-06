using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;

namespace SiteAdmin_API.Services.Interfaces
{
    public interface IPackageService
    {
        Task<ServiceResponse<Package>> AddUpdatePackage(AddUpdatePackageRequest request);
        Task<ServiceResponse<List<GetPackage>>> GetAllPackages();
        Task<ServiceResponse<bool>> UpdatePackageStatus(int packageId);
    }
}
