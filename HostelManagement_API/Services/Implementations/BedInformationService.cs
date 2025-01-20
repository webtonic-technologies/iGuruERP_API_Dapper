using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class BedInformationService : IBedInformationService
    {
        private readonly IBedInformationRepository _bedInformationRepository;

        public BedInformationService(IBedInformationRepository bedInformationRepository)
        {
            _bedInformationRepository = bedInformationRepository;
        }

        public async Task<IEnumerable<GetBedInformationResponse>> GetBedInformation(GetBedInformationRequest request)
        {
            return await _bedInformationRepository.GetBedInformation(request.InstituteID, request.HostelID, request.RoomID);
        }
    }
}
