using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IBuildingRepository
    {
        Task<int> AddUpdateBuilding(AddUpdateBuildingRequest request);
        Task<List<GetAllBuildingsResponse>> GetAllBuildings(GetAllBuildingsRequest request);  // Corrected return type
        Task<BuildingResponse> GetBuildingById(int buildingId);
        Task<int> DeleteBuilding(int buildingId);
    }
}
