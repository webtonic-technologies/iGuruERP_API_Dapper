using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Repository.Interfaces
{
    public interface ILeadStageRepository
    {
        Task<ServiceResponse<string>> AddUpdateLeadStage(LeadStage request);
        Task<ServiceResponse<List<LeadStage>>> GetAllLeadStages(GetAllRequest request);
        Task<ServiceResponse<LeadStage>> GetLeadStageById(int leadStageID);
        Task<ServiceResponse<bool>> UpdateLeadStageStatus(int leadStageID);
    }
}
