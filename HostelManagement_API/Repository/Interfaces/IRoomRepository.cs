using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IRoomRepository
    {
        Task<int> AddUpdateRoom(AddUpdateRoomRequest request);
        Task<PagedResponse<RoomResponse>> GetAllRooms(GetAllRoomsRequest request);
        Task<RoomResponse> GetRoomById(int roomId);
        Task<int> DeleteRoom(int roomId);
    }
}
