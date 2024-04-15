using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
using Employee_API.Models;
using Employee_API.Repository.Interfaces;
using Employee_API.Services.Interfaces;

namespace Employee_API.Services.Implementations
{
    public class EmployeeProfileServices : IEmployeeProfileServices
    {
        private readonly IEmployeeProfileRepository _employeeProfileRepository;

        public EmployeeProfileServices(IEmployeeProfileRepository employeeProfileRepository)
        {
            _employeeProfileRepository = employeeProfileRepository;
        }
        public async Task<ServiceResponse<string>> AddUpdateEmployeeProfile(EmployeeProfileDTO request)
        {
            try
            {
                return await _employeeProfileRepository.AddUpdateEmployeeProfile(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<EmployeeProfileResponseDTO>> GetEmployeeProfileById(int employeeId)
        {
            try
            {
                return await _employeeProfileRepository.GetEmployeeProfileById(employeeId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeProfileResponseDTO>(false, ex.Message, new EmployeeProfileResponseDTO(), 500);
            }
        }

        public async Task<ServiceResponse<List<EmployeeProfile>>> GetEmployeeProfileList(int InstituteId)
        {
            try
            {
                return await _employeeProfileRepository.GetEmployeeProfileList(InstituteId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeProfile>>(false, ex.Message, [], 500);
            }
        }
    }
}
