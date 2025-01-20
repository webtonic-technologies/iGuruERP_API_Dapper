using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IAllocationRepository
    {
        Task<ServiceResponse<IEnumerable<GetStudentResponse>>> GetStudentsByInstituteClassSection(GetStudentRequest request);
        Task<ServiceResponse<string>> AllocateHostel(AllocateHostelRequest request);
        Task<GetHostelHistoryResponse> GetHostelHistory(GetHostelHistoryRequest request);
        Task<ServiceResponse<string>> VacateHostel(VacateHostelRequest request);
        Task<IEnumerable<GetHostelResponse>> GetHostels(int instituteID);
        Task<IEnumerable<GetHostelRoomsResponse>> GetHostelRooms(int instituteID, int hostelID);
        Task<IEnumerable<GetHostelRoomBedsResponse>> GetHostelRoomBeds(int instituteID, int roomID);

    }
}
