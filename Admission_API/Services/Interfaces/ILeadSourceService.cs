using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Services.Interfaces
{
    public interface ILeadSourceService
    {
        Task<ServiceResponse<string>> AddUpdateLeadSource(LeadSource request);
        Task<ServiceResponse<List<LeadSource>>> GetAllLeadSources(GetAllRequest request);
        Task<ServiceResponse<LeadSource>> GetLeadSourceById(int leadSourceID);
        Task<ServiceResponse<bool>> UpdateLeadSourceStatus(int leadSourceID);
    }
}
