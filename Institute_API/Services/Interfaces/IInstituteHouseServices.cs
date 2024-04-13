using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Services.Interfaces
{
    public interface IInstituteHouseServices
    {
        Task<ServiceResponse<string>> AddUpdateInstituteHouse(InstituteHouseDTO request);
        Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseById(int Id);
        Task<ServiceResponse<byte[]>> GetInstituteHouseLogoById(int Id);
    }
}
