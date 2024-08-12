using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class BuildingService : IBuildingService
    {
        private readonly IBuildingRepository _buildingRepository;

        public BuildingService(IBuildingRepository buildingRepository)
        {
            _buildingRepository = buildingRepository;
        }

        public async Task<int> AddUpdateBuildings(AddUpdateBuildingsRequest request)
        {
            return await _buildingRepository.AddUpdateBuildings(request);
        }

        public async Task<ServiceResponse<List<GetAllBuildingsResponse>>> GetAllBuildings(GetAllBuildingsRequest request)
        {
            var buildings = await _buildingRepository.GetAllBuildings(request);
            return new ServiceResponse<List<GetAllBuildingsResponse>>(true, "Buildings retrieved successfully", buildings, 200);
        }

        public async Task<ServiceResponse<BuildingResponse>> GetBuildingById(int buildingId)
        {
            var building = await _buildingRepository.GetBuildingById(buildingId);
            return new ServiceResponse<BuildingResponse>(true, "Building retrieved successfully", building, 200);
        }

        public async Task<IEnumerable<BuildingResponse>> GetAllBuildingsFetch()
        {
            return await _buildingRepository.GetAllBuildingsFetch();
        }

        public async Task<ServiceResponse<int>> DeleteBuilding(int buildingId)
        {
            var result = await _buildingRepository.DeleteBuilding(buildingId);
            return new ServiceResponse<int>(true, "Building deleted successfully", result, 200);
        }
    }
}
