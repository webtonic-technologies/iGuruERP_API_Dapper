using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
using Employee_API.Repository.Interfaces;
using Employee_API.Services.Interfaces;

namespace Employee_API.Services.Implementations
{
    public class EmployeeLoginsServices: IEmployeeLoginsServices
    {
        private readonly IEmployeeLoginsRepository _employeeLoginsRepository;

        public EmployeeLoginsServices(IEmployeeLoginsRepository employeeLoginsRepository)
        {
            _employeeLoginsRepository = employeeLoginsRepository;
        }

        public async Task<ServiceResponse<byte[]>> DownloadExcelSheet(ExcelDownloadRequest request, string format)
        {
            try
            {
                return await _employeeLoginsRepository.DownloadExcelSheet(request, format);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<byte[]>> DownloadEmployeeActivity(DownloadExcelRequest request, string format)
        {
            try
            {
                return await _employeeLoginsRepository.DownloadEmployeeActivity(request, format);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<byte[]>> DownloadExcelSheetNonAppUsers(DownloadExcelRequest request, string format)
        {
            try
            {
                return await _employeeLoginsRepository.DownloadExcelSheetNonAppUsers(request, format);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<ForgetPasswordResponse>> ForgetPassword(ForgotPassword request)
        {
            return await _employeeLoginsRepository.ForgetPassword(request);
        }

        public async Task<ServiceResponse<List<EmployeeActivityResponse>>> GetAllEmployeeActivity(EmployeeLoginRequest request)
        {
            try
            {
                return await _employeeLoginsRepository.GetAllEmployeeActivity(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeActivityResponse>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<List<EmployeeCredentialsResponse>>> GetAllEmployeeLoginCred(EmployeeLoginRequest request)
        {
            try
            {
                return await _employeeLoginsRepository.GetAllEmployeeLoginCred(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeCredentialsResponse>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<List<EmployeeNonAppUsersResponse>>> GetAllEmployeeNonAppUsers(EmployeeLoginRequest request)
        {
            try
            {
                return await _employeeLoginsRepository.GetAllEmployeeNonAppUsers(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeNonAppUsersResponse>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<bool>> ResetPassword(ResetPassword request)
        {
            return await _employeeLoginsRepository.ResetPassword(request);
        }

        public async Task<ServiceResponse<EmployeeLoginResposne>> UserLogin(string username)
        {
            try
            {
                return await _employeeLoginsRepository.UserLogin(username);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeLoginResposne>(false, ex.Message, new EmployeeLoginResposne(), 500);
            }
        }

        public async Task<ServiceResponse<LoginResposne>> UserLoginPasswordScreen(UserLoginRequest request)
        {
            try
            {
                return await _employeeLoginsRepository.UserLoginPasswordScreen(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoginResposne>(false, ex.Message, new LoginResposne(), 500);
            }
        }

        public async Task<ServiceResponse<string>> UserLogout(string username)
        {
            return await _employeeLoginsRepository.UserLogout(username);
        }

        public async Task<ServiceResponse<UserSwitchOverResponse>> UserSwitchOver(UserSwitchOverRequest request)
        {
            return await _employeeLoginsRepository.UserSwitchOver(request);
        }
    }
}
