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

        public async Task<ServiceResponse<EmployeeSubstitutionResponse>> GetSubstitution(EmployeeSubstitutionRequest request)
        {
            // Your business logic here

            // Create a new EmployeeSubstitutionResponse object
            var response = new EmployeeSubstitutionResponse
            {
                // Populate the response with substitution details
            };

            // Return the response wrapped in a ServiceResponse
            return new ServiceResponse<EmployeeSubstitutionResponse>(
                true, "Substitution fetched successfully", response, 200);
        }


        public async Task<ServiceResponse<int>> UpdateSubstitution(EmployeeSubstitutionRequest_Update request)
        {
            return await _repository.UpdateSubstitution(request);
        }
    }
}
