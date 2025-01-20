using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Implementations;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IRoomTypeRepository _roomTypeRepository;

        public RoomTypeService(IRoomTypeRepository roomTypeRepository)
        {
            _roomTypeRepository = roomTypeRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateRoomType(AddUpdateRoomTypeRequest request)
        {
            var roomTypeId = await _roomTypeRepository.AddUpdateRoomType(request);
            return new ServiceResponse<int>(true, "Room type added/updated successfully", roomTypeId, 200);
        }
         
        public async Task<ServiceResponse<IEnumerable<RoomTypeResponse>>> GetAllRoomTypes(GetAllRoomTypesRequest request)
        {
            return await _roomTypeRepository.GetAllRoomTypes(request); 
        }

        public async Task<ServiceResponse<RoomTypeResponse>> GetRoomTypeById(int roomTypeId)
        {
            var roomType = await _roomTypeRepository.GetRoomTypeById(roomTypeId);
            return new ServiceResponse<RoomTypeResponse>(true, "Room type retrieved successfully", roomType, 200);
        }

        public async Task<ServiceResponse<int>> DeleteRoomType(int roomTypeId)
        {
            var result = await _roomTypeRepository.DeleteRoomType(roomTypeId);
            return new ServiceResponse<int>(true, "Room type deleted successfully", result, 200);
        }
    }
}
