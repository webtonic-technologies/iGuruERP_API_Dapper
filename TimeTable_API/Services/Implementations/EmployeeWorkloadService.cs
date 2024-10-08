using System.Threading.Tasks;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.Repository.Interfaces;
using TimeTable_API.Services.Interfaces;

namespace TimeTable_API.Services.Implementations
{
    public class EmployeeWorkloadService : IEmployeeWorkloadService
    {
        private readonly IEmployeeWorkloadRepository _repository;

        public EmployeeWorkloadService(IEmployeeWorkloadRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<EmployeeWorkloadResponse>> GetEmployeeWorkload(EmployeeWorkloadRequest request)
        {
            return await _repository.GetEmployeeWorkload(request);
        }
        public async Task<ServiceResponse<int>> AddUpdateWorkload(AddUpdateWorkloadRequest request)
        {
            return await _repository.AddUpdateWorkload(request);
        }
    }
}
