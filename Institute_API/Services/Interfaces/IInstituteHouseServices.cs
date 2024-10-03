using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Services.Interfaces
{
    public interface IInstituteHouseServices
    {
        Task<ServiceResponse<int>> AddUpdateInstituteHouse(InstituteHouseDTO request);
        Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseList(GetInstituteHouseList request);
        Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseById(int instituteHouseId);
        Task<ServiceResponse<bool>> SoftDeleteInstituteHouse(int instituteHouseId);
        Task<ServiceResponse<bool>> DeleteInstituteHouseImage(int instituteHouseId);
        Task<ServiceResponse<byte[]>> DownloadExcelSheet(int instituteId, string format);
    }
}
