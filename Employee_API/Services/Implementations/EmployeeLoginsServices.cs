using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
using Employee_API.Repository.Interfaces;
using Employee_API.Services.Interfaces;

namespace Employee_API.Services.Implementations
{
    public class EmployeeLoginsServices: IEmployeeLoginsServices
    {
        private readonly IEmployeeLoginsServices  _employeeLoginsServices;

        public EmployeeLoginsServices(IEmployeeLoginsServices employeeLoginsServices)
        {
            _employeeLoginsServices = employeeLoginsServices;
        }

        public async Task<ServiceResponse<byte[]>> DownloadExcelSheet(int InstituteId)
        {
            try
            {
                return await _employeeLoginsServices.DownloadExcelSheet(InstituteId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<byte[]>> DownloadExcelSheetEmployeeActivity(int InstituteId)
        {
            try
            {
                return await _employeeLoginsServices.DownloadExcelSheetEmployeeActivity(InstituteId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<byte[]>> DownloadExcelSheetNonAppUsers(int InstituteId)
        {
            try
            {
                return await _employeeLoginsServices.DownloadExcelSheetNonAppUsers(InstituteId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<List<EmployeeActivityResponse>>> GetAllEmployeeActivity(EmployeeLoginRequest request)
        {
            try
            {
                return await _employeeLoginsServices.GetAllEmployeeActivity(request);
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
                return await _employeeLoginsServices.GetAllEmployeeLoginCred(request);
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
                return await _employeeLoginsServices.GetAllEmployeeNonAppUsers(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeNonAppUsersResponse>>(false, ex.Message, [], 500);
            }
        }
    }
}
