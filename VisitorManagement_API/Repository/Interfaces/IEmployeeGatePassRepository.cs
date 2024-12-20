using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.Responses;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;

namespace VisitorManagement_API.Repository.Interfaces
{
    public interface IEmployeeGatePassRepository
    {
        Task<ServiceResponse<string>> AddUpdateEmployeeGatePass(EmployeeGatePass employeeGatePass);
        Task<ServiceResponse<IEnumerable<EmployeeGatepassResponse>>> GetAllEmployeeGatePass(GetAllEmployeeGatePassRequest request);
        Task<ServiceResponse<EmployeeGatepassResponse>> GetEmployeeGatePassById(int gatePassId);
        Task<ServiceResponse<bool>> UpdateEmployeeGatePassStatus(int gatePassId);
        Task<ServiceResponse<List<Visitedfor>>> GetAllVisitedForReason();
        Task<List<GetVisitorForDDLResponse>> GetVisitorForDDL();
        Task<GetGatePassSlipResponse> GetGatePassSlip(int gatePassID, int instituteID);
        Task<IEnumerable<GetEmployeeGatePassExportResponse>> GetEmployeeGatePassExport(GetEmployeeGatePassExportRequest request);

    }
}
 