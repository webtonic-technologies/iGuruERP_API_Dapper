using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Services.Interfaces
{
    public interface IInstituteHouseServices
    {
        Task<ServiceResponse<int>> AddUpdateInstituteHouse(InstituteHouseDTO request);
        Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseList(int Id, string searchText);
        Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseById(int instituteHouseId);
        Task<ServiceResponse<bool>> SoftDeleteInstituteHouse(int instituteHouseId);
    }
}
