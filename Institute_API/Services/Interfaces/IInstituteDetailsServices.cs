using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Services.Interfaces
{
    public interface IInstituteDetailsServices
    {
        Task<ServiceResponse<int>> AddUpdateInstititeDetails(InstituteDetailsDTO request);
        Task<ServiceResponse<InstituteDetailsDTO>> GetInstituteDetailsById(int Id);
        Task<ServiceResponse<byte[]>> GetInstituteLogoById(int Id);
        Task<ServiceResponse<byte[]>> GetInstituteDigitalStampById(int Id);
        Task<ServiceResponse<byte[]>> GetInstituteDigitalSignatoryById(int Id);
        Task<ServiceResponse<byte[]>> GetInstitutePrincipalSignatoryById(int Id);
    }
}
