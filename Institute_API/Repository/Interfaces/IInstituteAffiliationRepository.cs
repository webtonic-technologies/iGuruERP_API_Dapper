using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;

namespace Institute_API.Repository.Interfaces
{
    public interface IInstituteAffiliationRepository
    {
        Task<ServiceResponse<int>> AddUpdateInstituteAffiliation(AffiliationDTO request);
        Task<ServiceResponse<AffiliationDTO>> GetAffiliationInfoById(int Id);
    }
}
