using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
namespace Employee_API.Repository.Interfaces
{
    public interface IEmployeeLoginsRepository
    {
        Task<ServiceResponse<byte[]>> DownloadExcelSheet(int InstituteId);
        Task<ServiceResponse<byte[]>> DownloadExcelSheetNonAppUsers(int InstituteId);
        Task<ServiceResponse<byte[]>> DownloadExcelSheetEmployeeActivity(int InstituteId);
        Task<ServiceResponse<List<EmployeeCredentialsResponse>>> GetAllEmployeeLoginCred(EmployeeLoginRequest request);
        Task<ServiceResponse<List<EmployeeNonAppUsersResponse>>> GetAllEmployeeNonAppUsers(EmployeeLoginRequest request);
        Task<ServiceResponse<List<EmployeeActivityResponse>>> GetAllEmployeeActivity(EmployeeLoginRequest request);
    }
}
