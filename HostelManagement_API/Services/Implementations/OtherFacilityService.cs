using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class OtherFacilityService : IOtherFacilityService
    {
        private readonly IOtherFacilityRepository _otherFacilityRepository;

        public OtherFacilityService(IOtherFacilityRepository otherFacilityRepository)
        {
            _otherFacilityRepository = otherFacilityRepository;
        }

        public async Task<IEnumerable<FacilityResponse>> GetAllOtherFacilities()
        {
            return await _otherFacilityRepository.GetAllOtherFacilities();
        }
    }
}
