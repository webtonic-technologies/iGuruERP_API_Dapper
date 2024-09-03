using SiteAdmin_API.DTOs.Response;
using SiteAdmin_API.DTOs.ServiceResponse;

namespace SiteAdmin_API.Services.Interfaces
{
    public interface IPackageService_Institute
    {
        Task<ServiceResponse<List<PackageResponse_Institute>>> GetAllPackagesForInstitute();
    }
}
