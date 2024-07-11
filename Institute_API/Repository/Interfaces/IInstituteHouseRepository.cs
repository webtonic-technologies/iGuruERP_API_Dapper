using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;

namespace Institute_API.Repository.Interfaces
{
    public interface IInstituteHouseRepository
    {
        Task<ServiceResponse<int>> AddUpdateInstituteHouse(InstituteHouseDTO request);
        Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseList(int Id, string searchText);
        Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseById(int instituteHouseId);
        Task<ServiceResponse<bool>> SoftDeleteInstituteHouse(int instituteHouseId);
    }
}
