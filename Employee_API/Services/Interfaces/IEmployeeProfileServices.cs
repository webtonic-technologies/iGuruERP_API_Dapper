using Employee_API.DTOs.ServiceResponse;
using Employee_API.DTOs;
using Employee_API.Models;

namespace Employee_API.Services.Interfaces
{
    public interface IEmployeeProfileServices
    {
        Task<ServiceResponse<string>> AddUpdateEmployeeProfile(EmployeeProfileDTO request);
        Task<ServiceResponse<List<EmployeeProfile>>> GetEmployeeProfileList(int InstituteId);
        Task<ServiceResponse<EmployeeProfileResponseDTO>> GetEmployeeProfileById(int employeeId);
    }
}
