using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.Responses;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;
using VisitorManagement_API.Repository.Interfaces;
using VisitorManagement_API.Services.Interfaces;

namespace VisitorManagement_API.Services.Implementations
{
    public class VisitorLogService : IVisitorLogService
    {
        private readonly IVisitorLogRepository _visitorLogRepository;

        public VisitorLogService(IVisitorLogRepository visitorLogRepository)
        {
            _visitorLogRepository = visitorLogRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateVisitorLog(VisitorRequestDTO visitorLog)
        {
            return await _visitorLogRepository.AddUpdateVisitorLog(visitorLog);
        }

        public async Task<ServiceResponse<IEnumerable<Visitorlogresponse>>> GetAllVisitorLogs(GetAllVisitorLogsRequest request)
        {
            return await _visitorLogRepository.GetAllVisitorLogs(request);
        }

        public async Task<ServiceResponse<Visitorlogresponse>> GetVisitorLogById(int visitorId)
        {
            return await _visitorLogRepository.GetVisitorLogById(visitorId);
        }

        public async Task<ServiceResponse<bool>> UpdateVisitorLogStatus(int visitorId)
        {
            return await _visitorLogRepository.UpdateVisitorLogStatus(visitorId);
        }

        public async Task<ServiceResponse<IEnumerable<GetSourcesResponse>>> GetSources(GetSourcesRequest request)
        {
            return await _visitorLogRepository.GetSources(request);
        }
        public async Task<ServiceResponse<IEnumerable<GetPurposeResponse>>> GetPurpose(GetPurposeRequest request)
        {
            return await _visitorLogRepository.GetPurpose(request);
        }
        public async Task<ServiceResponse<IEnumerable<GetIDProofResponse>>> GetIDProof()
        {
            return await _visitorLogRepository.GetIDProof();
        }
        public async Task<ServiceResponse<IEnumerable<GetApprovalTypeResponse>>> GetApprovalType()
        {
            return await _visitorLogRepository.GetApprovalType();
        }

        public async Task<ServiceResponse<IEnumerable<GetEmployeeResponse>>> GetEmployee(GetEmployeeRequest request)
        {
            return await _visitorLogRepository.GetEmployee(request);
        }
        public async Task<ServiceResponse<GetVisitorSlipResponse>> GetVisitorSlip(GetVisitorSlipRequest request)
        {
            return await _visitorLogRepository.GetVisitorSlip(request);
        }

        public async Task<ServiceResponse<string>> ChangeApprovalStatus(ChangeApprovalStatusRequest request)
        {
            try
            {
                // Call repository to change approval status
                var result = await _visitorLogRepository.UpdateApprovalStatus(request.VisitorID, request.ApprovalTypeID, request.InstituteID);

                if (result)
                {
                    return new ServiceResponse<string>(true, "Approval status updated successfully.", null, 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Failed to update approval status.", null, 400);
                }
            }
            catch (System.Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }
    }
}
