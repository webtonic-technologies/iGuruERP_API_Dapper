using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;

namespace Institute_API.Repository.Interfaces
{
    public interface IInstituteHouseRepository
    {
        Task<ServiceResponse<string>> AddUpdateInstituteHouse(InstituteHouseDTO request);
        Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseById(int Id);
        Task<ServiceResponse<byte[]>> GetInstituteHouseLogoById(int Id);
    }
}
