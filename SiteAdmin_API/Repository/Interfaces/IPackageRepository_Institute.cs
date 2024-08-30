using SiteAdmin_API.DTOs.Response;
using SiteAdmin_API.DTOs.ServiceResponse;

namespace SiteAdmin_API.Repository.Interfaces
{
    public interface IPackageRepository_Institute
    {
        Task<ServiceResponse<List<PackageResponse_Institute>>> GetAllPackagesForInstitute();
    }
}
