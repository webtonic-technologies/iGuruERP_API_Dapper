using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;

namespace Institute_API.Repository.Interfaces
{
    public interface IInstituteDetailsRepository
    {
        Task<ServiceResponse<int>> AddUpdateInstititeDetails(InstituteDetailsDTO request);
        Task<ServiceResponse<InstituteDetailsDTO>> GetInstituteDetailsById(int Id);
        Task<ServiceResponse<byte[]>> GetInstituteLogoById(int Id);
        Task<ServiceResponse<byte[]>> GetInstituteDigitalStampById(int Id);
        Task<ServiceResponse<byte[]>> GetInstituteDigitalSignatoryById(int Id);
        Task<ServiceResponse<byte[]>> GetInstitutePrincipalSignatoryById(int Id);
        Task<ServiceResponse<string>> AddUpdateInstituteLogo(InstLogoDTO request);
        Task<ServiceResponse<string>> AddUpdatePrincipalSignatory(InstPriSignDTO request);
        Task<ServiceResponse<string>> AddUpdateDigitalSignatory(InstDigSignDTO request);
        Task<ServiceResponse<string>> AddUpdateDigitalStamp(InstDigiStampDTO request);
    }
}
