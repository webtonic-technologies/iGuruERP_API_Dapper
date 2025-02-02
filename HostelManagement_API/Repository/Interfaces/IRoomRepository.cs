using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IRoomRepository
    {
        Task<int> AddUpdateRoom(AddUpdateRoomRequest request);
        Task<ServiceResponse<IEnumerable<RoomResponse>>> GetAllRooms(GetAllRoomsRequest request);
        Task<RoomResponse> GetRoomById(int roomId);
        Task<int> DeleteRoom(int roomId);
        Task<IEnumerable<GetFloorsDDLResponse>> GetFloorsDDL(int instituteID);

    }
}
