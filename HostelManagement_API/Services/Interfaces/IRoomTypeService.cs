using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IRoomTypeService
    {
        Task<ServiceResponse<int>> AddUpdateRoomType(AddUpdateRoomTypeRequest request);
        Task<ServiceResponse<IEnumerable<RoomTypeResponse>>> GetAllRoomTypes(GetAllRoomTypesRequest request);
        Task<ServiceResponse<RoomTypeResponse>> GetRoomTypeById(int roomTypeId);
        Task<ServiceResponse<int>> DeleteRoomType(int roomTypeId);
    }
}
