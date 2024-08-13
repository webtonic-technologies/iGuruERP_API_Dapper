using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class RoomFacilityService : IRoomFacilityService
    {
        private readonly IRoomFacilityRepository _roomFacilityRepository;

        public RoomFacilityService(IRoomFacilityRepository roomFacilityRepository)
        {
            _roomFacilityRepository = roomFacilityRepository;
        }

        public async Task<IEnumerable<FacilityResponse>> GetAllRoomFacilities()
        {
            return await _roomFacilityRepository.GetAllRoomFacilities();
        }
    }
}
