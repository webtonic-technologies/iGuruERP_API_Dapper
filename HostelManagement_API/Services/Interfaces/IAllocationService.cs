using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IAllocationService
    {
        Task<ServiceResponse<IEnumerable<GetStudentResponse>>> GetStudents(GetStudentRequest request);
        Task<ServiceResponse<string>> AllocateHostel(AllocateHostelRequest request);
        Task<ServiceResponse<GetHostelHistoryResponse>> GetHostelHistory(GetHostelHistoryRequest request);
        Task<ServiceResponse<string>> VacateHostel(VacateHostelRequest request);
        Task<IEnumerable<GetHostelResponse>> GetHostel(GetHostelRequest request);
        Task<IEnumerable<GetHostelRoomsResponse>> GetHostelRooms(GetHostelRoomsRequest request);
        Task<IEnumerable<GetHostelRoomBedsResponse>> GetHostelRoomBeds(GetHostelRoomBedsRequest request);

    }
}
