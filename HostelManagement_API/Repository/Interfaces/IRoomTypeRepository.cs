using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;


namespace HostelManagement_API.Repository.Interfaces
{
    public interface IRoomTypeRepository
    {
        Task<int> AddUpdateRoomType(AddUpdateRoomTypeRequest request);
        Task<ServiceResponse<IEnumerable<RoomTypeResponse>>> GetAllRoomTypes(GetAllRoomTypesRequest request); 
        Task<RoomTypeResponse> GetRoomTypeById(int roomTypeId);
        Task<int> DeleteRoomType(int roomTypeId);
        Task<IEnumerable<GetRoomTypesDDLResponse>> GetRoomTypesDDL(int instituteID);

    }
}
