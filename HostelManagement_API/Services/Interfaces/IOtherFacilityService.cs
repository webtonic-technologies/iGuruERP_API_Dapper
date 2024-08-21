using HostelManagement_API.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IOtherFacilityService
    {
        Task<IEnumerable<FacilityResponse>> GetAllOtherFacilities();
    }
}
