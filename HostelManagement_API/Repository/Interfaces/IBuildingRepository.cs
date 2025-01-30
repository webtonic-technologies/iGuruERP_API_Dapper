using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using HostelManagement_API.DTOs.ServiceResponse;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IBuildingRepository
    {
        Task<ServiceResponse<string>> AddUpdateBuildings(AddUpdateBuildingsRequest request);
        Task<ServiceResponse<IEnumerable<GetAllBuildingsResponse>>> GetAllBuildings(GetAllBuildingsRequest request);
        Task<IEnumerable<BuildingFetchResponse>> GetAllBuildingsFetch(int instituteId);
        Task<BuildingResponse> GetBuildingById(int buildingId);
        Task<int> DeleteBuilding(int buildingId);
    }
}
