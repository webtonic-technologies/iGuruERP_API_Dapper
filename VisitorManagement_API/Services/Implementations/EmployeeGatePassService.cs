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
    public class EmployeeGatePassService : IEmployeeGatePassService
    {
        private readonly IEmployeeGatePassRepository _employeeGatePassRepository;

        public EmployeeGatePassService(IEmployeeGatePassRepository employeeGatePassRepository)
        {
            _employeeGatePassRepository = employeeGatePassRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateEmployeeGatePass(EmployeeGatePass employeeGatePass)
        {
            return await _employeeGatePassRepository.AddUpdateEmployeeGatePass(employeeGatePass);
        }

        public async Task<ServiceResponse<IEnumerable<EmployeeGatepassResponse>>> GetAllEmployeeGatePass(GetAllEmployeeGatePassRequest request)
        {
            return await _employeeGatePassRepository.GetAllEmployeeGatePass(request);
        }

        public async Task<ServiceResponse<List<Visitedfor>>> GetAllVisitedForReason()
        {
            return await _employeeGatePassRepository.GetAllVisitedForReason();
        }

        public async Task<ServiceResponse<EmployeeGatepassResponse>> GetEmployeeGatePassById(int gatePassId)
        {
            return await _employeeGatePassRepository.GetEmployeeGatePassById(gatePassId);
        }

        public async Task<ServiceResponse<bool>> UpdateEmployeeGatePassStatus(int gatePassId)
        {
            return await _employeeGatePassRepository.UpdateEmployeeGatePassStatus(gatePassId);
        }
        public async Task<List<GetVisitorForDDLResponse>> GetVisitorForDDL()
        {
            return await _employeeGatePassRepository.GetVisitorForDDL();
        }

        public async Task<GetGatePassSlipResponse> GetGatePassSlip(int gatePassID, int instituteID)
        {
            return await _employeeGatePassRepository.GetGatePassSlip(gatePassID, instituteID);
        }
    }
}
