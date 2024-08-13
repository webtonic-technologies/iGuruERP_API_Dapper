using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class BuildingFacilityService : IBuildingFacilityService
    {
        private readonly IBuildingFacilityRepository _buildingFacilityRepository;

        public BuildingFacilityService(IBuildingFacilityRepository buildingFacilityRepository)
        {
            _buildingFacilityRepository = buildingFacilityRepository;
        }

        public async Task<IEnumerable<FacilityResponse>> GetAllBuildingFacilities()
        {
            return await _buildingFacilityRepository.GetAllBuildingFacilities();
        }
    }
}
