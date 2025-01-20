using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IRoomService
    {
        Task<ServiceResponse<int>> AddUpdateRoom(AddUpdateRoomRequest request);
        Task<ServiceResponse<IEnumerable<RoomResponse>>> GetAllRooms(GetAllRoomsRequest request);
        Task<ServiceResponse<RoomResponse>> GetRoomById(int roomId);
        Task<ServiceResponse<bool>> DeleteRoom(int roomId);
    }
}
