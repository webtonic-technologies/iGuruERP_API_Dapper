using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IRoomTypeRepository
    {
        Task<int> AddUpdateRoomType(AddUpdateRoomTypeRequest request);
        Task<PagedResponse<RoomTypeResponse>> GetAllRoomTypes(GetAllRoomTypesRequest request);
        Task<RoomTypeResponse> GetRoomTypeById(int roomTypeId);
        Task<int> DeleteRoomType(int roomTypeId);
    }
}
