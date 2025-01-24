using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
namespace Employee_API.Repository.Interfaces
{
    public interface IEmployeeLoginsRepository
    {
        Task<ServiceResponse<byte[]>> DownloadExcelSheet(ExcelDownloadRequest request, string format);
        Task<ServiceResponse<byte[]>> DownloadExcelSheetNonAppUsers(DownloadExcelRequest request, string format);
        Task<ServiceResponse<byte[]>> DownloadEmployeeActivity(DownloadExcelRequest request, string format);
        Task<ServiceResponse<List<EmployeeCredentialsResponse>>> GetAllEmployeeLoginCred(EmployeeLoginRequest request);
        Task<ServiceResponse<List<EmployeeNonAppUsersResponse>>> GetAllEmployeeNonAppUsers(EmployeeLoginRequest request);
        Task<ServiceResponse<List<EmployeeActivityResponse>>> GetAllEmployeeActivity(EmployeeLoginRequest request);
        Task<ServiceResponse<EmployeeLoginResposne>> UserLogin(string username);
        Task<ServiceResponse<LoginResposne>> UserLoginPasswordScreen(UserLoginRequest request);
        Task<ServiceResponse<string>> UserLogout(string username);
        Task<ServiceResponse<UserSwitchOverResponse>> UserSwitchOver(UserSwitchOverRequest request);
        Task<ServiceResponse<ForgetPasswordResponse>> ForgetPassword(ForgotPassword request);
        Task<ServiceResponse<bool>> ResetPassword(ResetPassword request);
        Task<ServiceResponse<string>> CaptureDeviceDetails(DeviceDetails request);
    }
}
