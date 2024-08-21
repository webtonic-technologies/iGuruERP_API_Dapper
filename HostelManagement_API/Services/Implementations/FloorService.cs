using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class FloorService : IFloorService
    {
        private readonly IFloorRepository _floorRepository;

        public FloorService(IFloorRepository floorRepository)
        {
            _floorRepository = floorRepository;
        }

        public async Task<int> AddUpdateFloors(AddUpdateFloorsRequest request)
        {
            return await _floorRepository.AddUpdateFloors(request);
        }

        public async Task<ServiceResponse<List<GetAllFloorsResponse>>> GetAllFloors(GetAllFloorsRequest request)
        {
            var floors = await _floorRepository.GetAllFloors(request);
            return new ServiceResponse<List<GetAllFloorsResponse>>(true, "Floors retrieved successfully", floors, 200);
        }

        public async Task<ServiceResponse<FloorResponse>> GetFloorById(int floorId)
        {
            var floor = await _floorRepository.GetFloorById(floorId);
            return new ServiceResponse<FloorResponse>(true, "Floor retrieved successfully", floor, 200);
        }

        public async Task<ServiceResponse<int>> DeleteFloor(int floorId)
        {
            var result = await _floorRepository.DeleteFloor(floorId);
            return new ServiceResponse<int>(true, "Floor deleted successfully", result, 200);
        }
    }
}
