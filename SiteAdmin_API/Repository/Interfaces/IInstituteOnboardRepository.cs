using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Responses;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;

namespace SiteAdmin_API.Repository.Interfaces
{
    public interface IInstituteOnboardRepository
    {
        Task<ServiceResponse<InstituteOnboard>> AddUpdateInstituteOnboard(InstituteOnboardRequest request);
        Task<ServiceResponse<List<InstituteOnboard>>> GetAllInstituteOnboard(int pageNumber, int pageSize);
        Task<ServiceResponse<InstituteOnboard>> GetInstituteOnboardById(int instituteOnboardId);
        Task<ServiceResponse<string>> UpgradePackage(UpgradePackageRequest request);
        Task<ServiceResponse<List<GetPackageDDLResponse>>> GetPackageDDL();
        Task<ServiceResponse<GetAllInstituteInfoResponse>> GetAllInstituteInfo(int instituteOnboardId); 
        Task<ServiceResponse<int>> AddAdmissionURL(AddAdmissionURLRequest request);
        Task<ServiceResponse<IEnumerable<ActivityLogsResponse>>> GetActivityLogs(int instituteOnboardId);
        Task<ServiceResponse<CreateUserResponse>> CreateUserLoginInfo(CreateUserRequest request);
    }
}
