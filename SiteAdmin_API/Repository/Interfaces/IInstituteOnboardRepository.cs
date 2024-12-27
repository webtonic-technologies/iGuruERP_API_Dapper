using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Models;

namespace SiteAdmin_API.Repository.Interfaces
{
    public interface IInstituteOnboardRepository
    {
        Task<ServiceResponse<InstituteOnboard>> AddUpdateInstituteOnboard(InstituteOnboardRequest request);
        Task<ServiceResponse<List<InstituteOnboard>>> GetAllInstituteOnboard(int pageNumber, int pageSize);
        Task<ServiceResponse<InstituteOnboard>> GetInstituteOnboardById(int instituteOnboardId);
    }
}
