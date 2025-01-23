using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Repository.Interfaces
{
    public interface ILeadSourceRepository
    {
        Task<ServiceResponse<string>> AddUpdateLeadSource(List<LeadSource> request);  // Updated to accept List<LeadSource>
        Task<ServiceResponse<List<LeadSourceResponse>>> GetAllLeadSources(GetAllRequest request);
        Task<ServiceResponse<LeadSourceResponse>> GetLeadSourceById(int leadSourceID);
        Task<ServiceResponse<bool>> UpdateLeadSourceStatus(int leadSourceID);
    }
}
