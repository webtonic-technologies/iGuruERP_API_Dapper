using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;

namespace Institute_API.Repository.Interfaces
{
    public interface IInstituteAffiliationRepository
    {
        Task<ServiceResponse<int>> AddUpdateInstituteAffiliation(AffiliationDTO request);
        Task<ServiceResponse<string>> AddUpdateLogo(AffiliationLogoDTO request);
        Task<ServiceResponse<byte[]>> GetAffiliationLogoById(int Id);
        Task<ServiceResponse<AffiliationDTO>> GetAffiliationInfoById(int Id);
    }
}
