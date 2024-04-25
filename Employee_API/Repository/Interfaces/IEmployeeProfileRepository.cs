using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
using Employee_API.Models;

namespace Employee_API.Repository.Interfaces
{
    public interface IEmployeeProfileRepository
    {
        Task<ServiceResponse<int>> AddUpdateEmployeeProfile(EmployeeProfileDTO request);
        Task<ServiceResponse<int>> AddUpdateEmployeeFamily(EmployeeFamily request);
        Task<ServiceResponse<string>> AddUpdateEmployeeDecuments(EmployeeDocumentDTO request, int employeeId);
        Task<ServiceResponse<int>> AddUpdateEmployeeQualification(List<EmployeeQualification>? request, int employeeId);
        Task<ServiceResponse<int>> AddUpdateEmployeeWorkExp(List<EmployeeWorkExperience>? request, int employeeId);
        Task<ServiceResponse<int>> AddUpdateEmployeeBankDetails(List<EmployeeBankDetails>? request, int employeeId);
        Task<ServiceResponse<List<EmployeeProfile>>> GetEmployeeProfileList(GetAllEmployeeListRequest request);
        Task<ServiceResponse<EmployeeProfileResponseDTO>> GetEmployeeProfileById(int employeeId);
        Task<ServiceResponse<List<byte[]>>> GetEmployeeDocuments(int employeeId);
        Task<ServiceResponse<EmployeeFamily>> GetEmployeeFamilyDetailsById(int employeeId);
        Task<ServiceResponse<List<EmployeeQualification>>> GetEmployeeQualificationById(int employeeId);
        Task<ServiceResponse<List<EmployeeWorkExperience>>> GetEmployeeWorkExperienceById(int employeeId);
        Task<ServiceResponse<List<EmployeeBankDetails>>> GetEmployeeBankDetailsById(int employeeId);







    }
}
