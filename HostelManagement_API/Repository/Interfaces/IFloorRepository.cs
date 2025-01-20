using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IFloorRepository
    {
        Task<ServiceResponse<string>> AddUpdateFloors(AddUpdateFloorsRequest request); 
        Task<ServiceResponse<IEnumerable<GetAllFloorsResponse>>> GetAllFloors(GetAllFloorsRequest request);
        Task<FloorResponse> GetFloorById(int floorId);
        Task<int> DeleteFloor(int floorId);
    }
}
