using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IBedInformationService
    {
        Task<IEnumerable<GetBedInformationResponse>> GetBedInformation(GetBedInformationRequest request);
    }
}
