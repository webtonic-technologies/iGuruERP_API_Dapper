﻿using HostelManagement_API.DTOs.Requests;
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
         

        public async Task<ServiceResponse<string>>  AddUpdateBuildings(AddUpdateBuildingsRequest request)
        {
            return await _buildingRepository.AddUpdateBuildings(request);
        }
         
        public async Task<ServiceResponse<IEnumerable<GetAllBuildingsResponse>>> GetAllBuildings(GetAllBuildingsRequest request)
        { 
            return await _buildingRepository.GetAllBuildings(request); 
        }

        public async Task<ServiceResponse<BuildingResponse>> GetBuildingById(int buildingId)
        {
            var building = await _buildingRepository.GetBuildingById(buildingId);
            return new ServiceResponse<BuildingResponse>(true, "Building retrieved successfully", building, 200);
        }

        public async Task<IEnumerable<BuildingFetchResponse>> GetAllBuildingsFetch(int instituteId)
        {
            return await _buildingRepository.GetAllBuildingsFetch(instituteId);
        }


        public async Task<ServiceResponse<int>> DeleteBuilding(int buildingId)
        {
            var result = await _buildingRepository.DeleteBuilding(buildingId);
            return new ServiceResponse<int>(true, "Building deleted successfully", result, 200);
        }
    }
}
