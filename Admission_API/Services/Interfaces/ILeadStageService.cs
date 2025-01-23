using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Services.Interfaces
{
    public interface ILeadStageService
    {
        //Task<ServiceResponse<string>> AddUpdateLeadStage(LeadStage request);
        Task<ServiceResponse<string>> AddUpdateLeadStage(List<LeadStage> request);

        Task<ServiceResponse<List<LeadStage>>> GetAllLeadStages(GetAllRequest request);
        Task<ServiceResponse<LeadStage>> GetLeadStageById(int leadStageID);
        Task<ServiceResponse<bool>> UpdateLeadStageStatus(int leadStageID);
    }
}
