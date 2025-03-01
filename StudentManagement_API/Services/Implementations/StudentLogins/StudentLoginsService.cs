using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;
using StudentManagement_API.Repository.Interfaces;
using StudentManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagement_API.Services.Implementations
{
    public class StudentLoginsService : IStudentLoginsService
    {
        private readonly IStudentLoginsRepository _repository;

        public StudentLoginsService(IStudentLoginsRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<List<GetLoginCredentialsResponse>>> GetLoginCredentialsAsync(GetLoginCredentialsRequest request)
        {
            return await _repository.GetLoginCredentialsAsync(request);
        }

        public async Task<ServiceResponse<string>> GetLoginCredentialsExportAsync(GetLoginCredentialsExportRequest request)
        {
            return await _repository.GetLoginCredentialsExportAsync(request);
        }

        public async Task<ServiceResponse<List<GetNonAppUsersResponse>>> GetNonAppUsersAsync(GetNonAppUsersRequest request)
        {
            return await _repository.GetNonAppUsersAsync(request);
        }

        public async Task<ServiceResponse<string>> GetNonAppUsersExportAsync(GetNonAppUsersExportRequest request)
        {
            return await _repository.GetNonAppUsersExportAsync(request);
        }
    }
}
