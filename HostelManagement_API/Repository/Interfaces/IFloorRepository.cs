using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IFloorRepository
    {
        Task<int> AddUpdateFloors(AddUpdateFloorsRequest request);
        Task<List<GetAllFloorsResponse>> GetAllFloors(GetAllFloorsRequest request);
        Task<FloorResponse> GetFloorById(int floorId);
        Task<int> DeleteFloor(int floorId);
    }
}
