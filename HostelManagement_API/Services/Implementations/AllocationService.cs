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
    public class AllocationService : IAllocationService
    {
        private readonly IAllocationRepository _allocationRepository;

        public AllocationService(IAllocationRepository allocationRepository)
        {
            _allocationRepository = allocationRepository;
        }

        public async Task<ServiceResponse<IEnumerable<GetStudentResponse>>> GetStudents(GetStudentRequest request)
        {

            return await _allocationRepository.GetStudentsByInstituteClassSection(request); 
        }

        public async Task<ServiceResponse<string>> AllocateHostel(AllocateHostelRequest request)
        {
            var result = await _allocationRepository.AllocateHostel(request);
            return result;
        }

        public async Task<ServiceResponse<GetHostelHistoryResponse>> GetHostelHistory(GetHostelHistoryRequest request)
        {
            var result = await _allocationRepository.GetHostelHistory(request);

            return new ServiceResponse<GetHostelHistoryResponse>(
                success: true,
                message: "Hostel history retrieved successfully",
                data: result,
                statusCode: 200
            );
        }

        public async Task<ServiceResponse<string>> VacateHostel(VacateHostelRequest request)
        {
            return await _allocationRepository.VacateHostel(request);
        }

        public async Task<IEnumerable<GetHostelResponse>> GetHostel(GetHostelRequest request)
        {
            return await _allocationRepository.GetHostels(request.InstituteID);
        }

        public async Task<IEnumerable<GetHostelRoomsResponse>> GetHostelRooms(GetHostelRoomsRequest request)
        {
            return await _allocationRepository.GetHostelRooms(request.InstituteID, request.HostelID);
        }

        public async Task<IEnumerable<GetHostelRoomBedsResponse>> GetHostelRoomBeds(GetHostelRoomBedsRequest request)
        {
            return await _allocationRepository.GetHostelRoomBeds(request.InstituteID, request.RoomID);
        }
    }
}
