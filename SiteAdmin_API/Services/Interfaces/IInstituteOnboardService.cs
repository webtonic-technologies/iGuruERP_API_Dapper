using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Responses;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;

namespace SiteAdmin_API.Services.Interfaces
{
    public interface IInstituteOnboardService
    {
        Task<ServiceResponse<InstituteOnboard>> AddUpdateInstituteOnboard(InstituteOnboardRequest request);
        Task<ServiceResponse<List<InstituteOnboard>>> GetAllInstituteOnboard(int pageNumber, int pageSize);
        Task<ServiceResponse<InstituteOnboard>> GetInstituteOnboardById(int instituteOnboardId);
        Task<ServiceResponse<string>> UpgradePackage(UpgradePackageRequest request);
        Task<ServiceResponse<List<GetPackageDDLResponse>>> GetPackageDDL();
        Task<ServiceResponse<GetAllInstituteInfoResponse>> GetAllInstituteInfo(GetAllInstituteInfoRequest request);
        Task<ServiceResponse<int>> AddAdmissionURL(AddAdmissionURLRequest request);
        Task<ServiceResponse<IEnumerable<ActivityLogsResponse>>> GetActivityLogs(ActivityLogsRequest request);
        Task<ServiceResponse<CreateUserResponse>> CreateUserLoginInfo(CreateUserRequest request);
    }
}
