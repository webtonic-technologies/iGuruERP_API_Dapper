using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using Admission_API.Repository.Interfaces;
using Admission_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Services.Implementations
{
    public class LeadStageService : ILeadStageService
    {
        private readonly ILeadStageRepository _repository;

        public LeadStageService(ILeadStageRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<string>> AddUpdateLeadStage(LeadStage request)
        {
            return await _repository.AddUpdateLeadStage(request);
        }

        public async Task<ServiceResponse<List<LeadStage>>> GetAllLeadStages(GetAllRequest request)
        {
            return await _repository.GetAllLeadStages(request);
        }

        public async Task<ServiceResponse<LeadStage>> GetLeadStageById(int leadStageID)
        {
            return await _repository.GetLeadStageById(leadStageID);
        }

        public async Task<ServiceResponse<bool>> UpdateLeadStageStatus(int leadStageID)
        {
            return await _repository.UpdateLeadStageStatus(leadStageID);
        }
    }
}
