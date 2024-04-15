using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Services.Interfaces
{
    public interface IInstituteHouseServices
    {
        Task<ServiceResponse<int>> AddUpdateInstituteHouse(InstituteHouseDTO request);
        Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseList(int Id);
        Task<ServiceResponse<byte[]>> GetInstituteHouseLogoById(int Id);
        Task<ServiceResponse<string>> AddUpdateHouseFile(HoueseFile request);
    }
}
