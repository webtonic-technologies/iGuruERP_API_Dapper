using System.Threading.Tasks;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.Repository.Interfaces;
using TimeTable_API.Services.Interfaces;

namespace TimeTable_API.Services.Implementations
{
    public class EmployeeSubstitutionService : IEmployeeSubstitutionService
    {
        private readonly IEmployeeSubstitutionRepository _repository;

        public EmployeeSubstitutionService(IEmployeeSubstitutionRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<List<EmployeeSubstitutionResponse>>> GetSubstitution(EmployeeSubstitutionRequest request)
        {
            return await _repository.GetSubstitution(request);
        }


        public async Task<ServiceResponse<int>> UpdateSubstitution(EmployeeSubstitutionRequest_Update request)
        {
            return await _repository.UpdateSubstitution(request);
        }
        public async Task<ServiceResponse<List<SubstituteEmployeeResponse>>> GetSubstituteEmployeeList(GetSubstituteEmployeeListRequest request)
        {
            return await _repository.GetSubstituteEmployeeList(request);
        }
    }
}
