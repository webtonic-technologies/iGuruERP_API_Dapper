using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagement_API.Services.Interfaces
{
    public interface IStudentLoginsService
    {
        Task<ServiceResponse<List<GetLoginCredentialsResponse>>> GetLoginCredentialsAsync(GetLoginCredentialsRequest request);
        Task<ServiceResponse<string>> GetLoginCredentialsExportAsync(GetLoginCredentialsExportRequest request); 
        Task<ServiceResponse<List<GetNonAppUsersResponse>>> GetNonAppUsersAsync(GetNonAppUsersRequest request);
        Task<ServiceResponse<string>> GetNonAppUsersExportAsync(GetNonAppUsersExportRequest request);


    }
}
