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

        public async Task<ServiceResponse<int>> AddUpdateEmployeeDocuments(List<EmployeeDocument> request, int employeeId)
        {
            try
            {
                return await _employeeProfileRepository.AddUpdateEmployeeDocuments(request, employeeId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
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

        public async Task<ServiceResponse<ClassSectionSubjectResponse>> ClassSectionSubjectsList(int classId, int sectionId)
        {
            return await _employeeProfileRepository.ClassSectionSubjectsList(classId, sectionId);
        }

        public async Task<ServiceResponse<List<ClassSectionSubjectResponse>>> ClassSectionSubjectsMappings(int InstituteId)
        {
            return await _employeeProfileRepository.ClassSectionSubjectsMappings(InstituteId);
        }

        public async Task<ServiceResponse<byte[]>> ExcelDownload(ExcelDownloadRequest request)
        {
            return await _employeeProfileRepository.ExcelDownload(request);
        }

        public async Task<ServiceResponse<List<BloodGroup>>> GetBloodGroupList()
        {
            try
            {
                return await _employeeProfileRepository.GetBloodGroupList();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<BloodGroup>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<List<ClassSectionList>>> GetClassSectionList(int instituteId)
        {
            return await _employeeProfileRepository.GetClassSectionList(instituteId);
        }

        public async Task<ServiceResponse<List<Department>>> GetDepartmentList(int InstituteId)
        {
            try
            {
                return await _employeeProfileRepository.GetDepartmentList(InstituteId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Department>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<List<Designation>>> GetDesignationList(int DepartmentId)
        {
            try
            {
                return await _employeeProfileRepository.GetDesignationList(DepartmentId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Designation>>(false, ex.Message, [], 500);
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

        public async Task<ServiceResponse<List<EmployeeProfileResponseDTO>>> GetEmployeeProfileList(GetAllEmployeeListRequest request)
        {
            try
            {
                return await _employeeProfileRepository.GetEmployeeProfileList(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeProfileResponseDTO>>(false, ex.Message, [], 500);
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

        public async Task<ServiceResponse<List<MaritalStatus>>> GetMaritalStatusList()
        {
            try
            {
                return await _employeeProfileRepository.GetMaritalStatusList();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<MaritalStatus>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<bool>> StatusActiveInactive(int employeeId)
        {
            try
            {
                return await _employeeProfileRepository.StatusActiveInactive(employeeId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdatePassword(ChangePasswordRequest request)
        {
            try
            {
                return await _employeeProfileRepository.UpdatePassword(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
