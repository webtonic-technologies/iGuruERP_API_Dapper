using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IFloorService
    {  
        Task<ServiceResponse<string>> AddUpdateFloors(AddUpdateFloorsRequest request); 
        Task<ServiceResponse<IEnumerable<GetAllFloorsResponse>>> GetAllFloors(GetAllFloorsRequest request);
        Task<ServiceResponse<FloorResponse>> GetFloorById(int floorId);
        Task<ServiceResponse<int>> DeleteFloor(int floorId);
    }
}
