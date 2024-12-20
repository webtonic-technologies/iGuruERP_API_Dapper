using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.Responses;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;

namespace VisitorManagement_API.Services.Interfaces
{
    public interface IVisitorLogService
    {
        Task<ServiceResponse<string>> AddUpdateVisitorLog(VisitorRequestDTO visitorLog);
        Task<ServiceResponse<IEnumerable<Visitorlogresponse>>> GetAllVisitorLogs(GetAllVisitorLogsRequest request);
        Task<ServiceResponse<Visitorlogresponse>> GetVisitorLogById(int visitorId);
        Task<ServiceResponse<bool>> UpdateVisitorLogStatus(int visitorId);
        Task<ServiceResponse<IEnumerable<GetSourcesResponse>>> GetSources(GetSourcesRequest request); 
        Task<ServiceResponse<IEnumerable<GetPurposeResponse>>> GetPurpose(GetPurposeRequest request);  // New method
        Task<ServiceResponse<IEnumerable<GetIDProofResponse>>> GetIDProof();
        Task<ServiceResponse<IEnumerable<GetApprovalTypeResponse>>> GetApprovalType();
        Task<ServiceResponse<IEnumerable<GetEmployeeResponse>>> GetEmployee(GetEmployeeRequest request);
        Task<ServiceResponse<GetVisitorSlipResponse>> GetVisitorSlip(GetVisitorSlipRequest request);
        Task<ServiceResponse<string>> ChangeApprovalStatus(ChangeApprovalStatusRequest request);


    }
}
