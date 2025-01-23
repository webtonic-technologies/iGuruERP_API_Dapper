using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using Admission_API.Repository.Interfaces;
using Admission_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Services.Implementations
{
    public class LeadSourceService : ILeadSourceService
    {
        private readonly ILeadSourceRepository _repository;

        public LeadSourceService(ILeadSourceRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<string>> AddUpdateLeadSource(List<LeadSource> request)
        {
            return await _repository.AddUpdateLeadSource(request);
        }


        public async Task<ServiceResponse<List<LeadSourceResponse>>> GetAllLeadSources(GetAllRequest request)
        {
            return await _repository.GetAllLeadSources(request);
        }

        public async Task<ServiceResponse<LeadSourceResponse>> GetLeadSourceById(int leadSourceID)
        {
            return await _repository.GetLeadSourceById(leadSourceID);
        }

        public async Task<ServiceResponse<bool>> UpdateLeadSourceStatus(int leadSourceID)
        {
            return await _repository.UpdateLeadSourceStatus(leadSourceID);
        }
    }
}
