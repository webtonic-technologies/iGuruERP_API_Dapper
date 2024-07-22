using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;

namespace Institute_API.Repository.Interfaces
{
    public interface IInstituteHouseRepository
    {
        Task<ServiceResponse<int>> AddUpdateInstituteHouse(InstituteHouseDTO request);
        Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseList(GetInstituteHouseList request);
        Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseById(int instituteHouseId);
        Task<ServiceResponse<bool>> SoftDeleteInstituteHouse(int instituteHouseId);
        Task<ServiceResponse<bool>> DeleteInstituteHouseImage(int instituteHouseId);
    }
}
