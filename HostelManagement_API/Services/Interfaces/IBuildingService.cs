using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IBuildingService
    { 
        Task<ServiceResponse<string>> AddUpdateBuildings(AddUpdateBuildingsRequest request);
        Task<ServiceResponse<IEnumerable<GetAllBuildingsResponse>>> GetAllBuildings(GetAllBuildingsRequest request);
        Task<IEnumerable<BuildingFetchResponse>> GetAllBuildingsFetch(int instituteId);
        Task<ServiceResponse<BuildingResponse>> GetBuildingById(int buildingId);
        Task<ServiceResponse<int>> DeleteBuilding(int buildingId);
    }
}
