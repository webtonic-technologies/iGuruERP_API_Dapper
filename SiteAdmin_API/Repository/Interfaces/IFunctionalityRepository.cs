using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;

namespace SiteAdmin_API.Repository.Interfaces
{
    public interface IFunctionalityRepository
    {
        Task<ServiceResponse<bool>> UpdateFunctionality(UpdateFunctionalityRequest request);
        Task<ServiceResponse<bool>> UpdateFunctionalityStatus(int functionalityId);
    }
}
