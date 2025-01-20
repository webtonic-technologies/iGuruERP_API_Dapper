using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Implementations;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateRoom(AddUpdateRoomRequest request)
        {
            var roomId = await _roomRepository.AddUpdateRoom(request);
            return new ServiceResponse<int>(true, "Room added/updated successfully", roomId, 200);
        }

        public async Task<ServiceResponse<IEnumerable<RoomResponse>>> GetAllRooms(GetAllRoomsRequest request)
        { 
            return await _roomRepository.GetAllRooms(request);

        }

        public async Task<ServiceResponse<RoomResponse>> GetRoomById(int roomId)
        {
            var room = await _roomRepository.GetRoomById(roomId);
            if (room == null)
            {
                return new ServiceResponse<RoomResponse>(false, "Room not found", null, 404);
            }
            return new ServiceResponse<RoomResponse>(true, "Room retrieved successfully", room, 200);
        }

        public async Task<ServiceResponse<bool>> DeleteRoom(int roomId)
        {
            var result = await _roomRepository.DeleteRoom(roomId);
            return new ServiceResponse<bool>(result > 0, result > 0 ? "Room deleted successfully" : "Room not found", result > 0, result > 0 ? 200 : 404);
        }
    }
}
