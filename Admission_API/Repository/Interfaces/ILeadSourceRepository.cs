using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Repository.Interfaces
{
    public interface ILeadSourceRepository
    {
        Task<ServiceResponse<string>> AddUpdateLeadSource(LeadSource request);
        Task<ServiceResponse<List<LeadSource>>> GetAllLeadSources(GetAllRequest request);
        Task<ServiceResponse<LeadSource>> GetLeadSourceById(int leadSourceID);
        Task<ServiceResponse<bool>> UpdateLeadSourceStatus(int leadSourceID);
    }
}
