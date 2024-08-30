using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IBuildingService
    {
        Task<int> AddUpdateBuildings(AddUpdateBuildingsRequest request);
        Task<ServiceResponse<List<GetAllBuildingsResponse>>> GetAllBuildings(GetAllBuildingsRequest request);  // Corrected return type
        Task<IEnumerable<BuildingResponse>> GetAllBuildingsFetch();
        Task<ServiceResponse<BuildingResponse>> GetBuildingById(int buildingId);
        Task<ServiceResponse<int>> DeleteBuilding(int buildingId);
    }
}
