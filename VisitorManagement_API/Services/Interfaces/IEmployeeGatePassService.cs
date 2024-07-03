using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;

namespace VisitorManagement_API.Services.Interfaces
{
    public interface IEmployeeGatePassService
    {
        Task<ServiceResponse<string>> AddUpdateEmployeeGatePass(EmployeeGatePass employeeGatePass);
        Task<ServiceResponse<IEnumerable<EmployeeGatePass>>> GetAllEmployeeGatePass(GetAllEmployeeGatePassRequest request);
        Task<ServiceResponse<EmployeeGatePass>> GetEmployeeGatePassById(int gatePassId);
        Task<ServiceResponse<bool>> UpdateEmployeeGatePassStatus(int gatePassId);
    }
}
