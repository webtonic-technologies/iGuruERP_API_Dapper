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

        public async Task<ServiceResponse<int>> AddUpdateEmployeeBankDetails(List<EmployeeBankDetails>? request, int employeeId)
        {
            try
            {
                return await _employeeProfileRepository.AddUpdateEmployeeBankDetails(request, employeeId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<string>> AddUpdateEmployeeDocuments(List<EmployeeDocument> request, int employeeId)
        {
            try
            {
                return await _employeeProfileRepository.AddUpdateEmployeeDocuments(request, employeeId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<int>> AddUpdateEmployeeFamily(EmployeeFamily request)
        {
            try
            {
                return await _employeeProfileRepository.AddUpdateEmployeeFamily(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<int>> AddUpdateEmployeeProfile(EmployeeProfile request)
        {
            try
            {
                return await _employeeProfileRepository.AddUpdateEmployeeProfile(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<int>> AddUpdateEmployeeQualification(List<EmployeeQualification>? request, int employeeId)
        {
            try
            {
                return await _employeeProfileRepository.AddUpdateEmployeeQualification(request, employeeId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<int>> AddUpdateEmployeeWorkExp(List<EmployeeWorkExperience>? request, int employeeId)
        {
            try
            {
                return await _employeeProfileRepository.AddUpdateEmployeeWorkExp(request, employeeId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<List<EmployeeBankDetails>>> GetEmployeeBankDetailsById(int employeeId)
        {
            try
            {
                return await _employeeProfileRepository.GetEmployeeBankDetailsById(employeeId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeBankDetails>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<List<EmployeeDocument>>> GetEmployeeDocuments(int employeeId)
        {
            try
            {
                return await _employeeProfileRepository.GetEmployeeDocuments(employeeId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeDocument>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<EmployeeFamily>> GetEmployeeFamilyDetailsById(int employeeId)
        {
            try
            {
                return await _employeeProfileRepository.GetEmployeeFamilyDetailsById(employeeId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeFamily>(false, ex.Message, new EmployeeFamily(), 500);
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

        public async Task<ServiceResponse<List<EmployeeProfile>>> GetEmployeeProfileList(GetAllEmployeeListRequest request)
        {
            try
            {
                return await _employeeProfileRepository.GetEmployeeProfileList(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeProfile>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<List<EmployeeQualification>>> GetEmployeeQualificationById(int employeeId)
        {
            try
            {
                return await _employeeProfileRepository.GetEmployeeQualificationById(employeeId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeQualification>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<List<EmployeeWorkExperience>>> GetEmployeeWorkExperienceById(int employeeId)
        {
            try
            {
                return await _employeeProfileRepository.GetEmployeeWorkExperienceById(employeeId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeWorkExperience>>(false, ex.Message, [], 500);
            }
        }
    }
}
