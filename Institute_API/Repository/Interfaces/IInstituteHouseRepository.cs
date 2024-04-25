using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;

namespace Institute_API.Repository.Interfaces
{
    public interface IInstituteHouseRepository
    {
        Task<ServiceResponse<int>> AddUpdateInstituteHouse(InstituteHouseDTO request);
        Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseList(int Id);
        Task<ServiceResponse<byte[]>> GetInstituteHouseLogoById(int Id);
        Task<ServiceResponse<string>> AddUpdateHouseFile(HoueseFile request);
    }
}
