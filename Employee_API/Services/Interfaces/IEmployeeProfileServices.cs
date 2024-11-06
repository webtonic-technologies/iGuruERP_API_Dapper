using Employee_API.DTOs.ServiceResponse;
using Employee_API.DTOs;
using Employee_API.Models;

namespace Employee_API.Services.Interfaces
{
    public interface IEmployeeProfileServices
    {
        Task<ServiceResponse<int>> AddUpdateEmployeeProfile(EmployeeProfile request);
        Task<ServiceResponse<int>> AddUpdateEmployeeFamily(EmployeeFamily request);
        Task<ServiceResponse<int>> AddUpdateEmployeeDocuments(List<EmployeeDocument> request, int employeeId);
        Task<ServiceResponse<int>> AddUpdateEmployeeQualification(List<EmployeeQualification>? request, int employeeId);
        Task<ServiceResponse<int>> AddUpdateEmployeeWorkExp(List<EmployeeWorkExperience>? request, int employeeId);
        Task<ServiceResponse<int>> AddUpdateEmployeeBankDetails(List<EmployeeBankDetails>? request, int employeeId);
        Task<ServiceResponse<IEnumerable<dynamic>>> GetEmployeeProfileList(GetAllEmployeeListRequest request);
        Task<ServiceResponse<EmployeeProfileResponseDTO>> GetEmployeeProfileById(int employeeId);
        Task<ServiceResponse<List<EmployeeDocument>>> GetEmployeeDocuments(int employeeId);
        Task<ServiceResponse<EmployeeFamily>> GetEmployeeFamilyDetailsById(int employeeId);
        Task<ServiceResponse<List<EmployeeQualification>>> GetEmployeeQualificationById(int employeeId);
        Task<ServiceResponse<List<EmployeeWorkExperience>>> GetEmployeeWorkExperienceById(int employeeId);
        Task<ServiceResponse<List<EmployeeBankDetails>>> GetEmployeeBankDetailsById(int employeeId);
        Task<ServiceResponse<bool>> StatusActiveInactive(EmployeeStatusRequest request);
        Task<ServiceResponse<List<MaritalStatus>>> GetMaritalStatusList();
        Task<ServiceResponse<List<BloodGroup>>> GetBloodGroupList();
        Task<ServiceResponse<List<Department>>> GetDepartmentList(int InstituteId);
        Task<ServiceResponse<List<Designation>>> GetDesignationList(int DepartmentId);
        Task<ServiceResponse<bool>> UpdatePassword(ChangePasswordRequest request);
        Task<ServiceResponse<byte[]>> ExcelDownload(ExcelDownloadRequest request, string format);
        Task<ServiceResponse<List<ClassSectionList>>> GetClassSectionList(int instituteId);
        Task<ServiceResponse<ClassSectionSubjectResponse>> ClassSectionSubjectsList(int classId, int sectionId);
        Task<ServiceResponse<List<ClassSectionSubjectResponse>>> ClassSectionSubjectsMappings(int InstituteId);
        Task<ServiceResponse<byte[]>> BulkUpdate(GetListRequest request);
        Task<ServiceResponse<string>> BulkUpdateEmployee(List<EmployeeProfile> request, string IpAddress);
        Task<ServiceResponse<IEnumerable<CategoryWiseEmployeeColumns>>> GetEmployeeColumnsAsync();
        Task<IEnumerable<dynamic>> ParseExcelFile(IFormFile file, int instituteId);
        Task<ServiceResponse<IEnumerable<EmployeeExportHistoryDto>>> GetBulkHistoryByInstituteId(int instituteId);
        Task<ServiceResponse<IEnumerable<EmployeeExportHistoryDto>>> GetImportHistoryByInstituteId(int instituteId);
        Task<ServiceResponse<IEnumerable<EmployeeExportHistoryDto>>> GetExportHistoryByInstituteId(int instituteId);
        Task<ServiceResponse<byte[]>> DownloadSheetImport(int InstituteId);
        Task<ServiceResponse<int>> UploadEmployeedata(IFormFile file, int instituteId, string IpAddress);
    }
}
