using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using Admission_API.Repository.Interfaces;
using Admission_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Services.Implementations
{
    public class RegistrationSetupService : IRegistrationSetupService
    {
        private readonly IRegistrationSetupRepository _repository;

        public RegistrationSetupService(IRegistrationSetupRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<string>> AddUpdateRegistrationSetup(RegistrationSetup request)
        {
            return await _repository.AddUpdateRegistrationSetup(request);
        }

        public async Task<ServiceResponse<List<RegistrationSetup>>> GetAllRegistrationSetups(GetAllRequest request)
        {
            return await _repository.GetAllRegistrationSetups(request);
        }

        public async Task<ServiceResponse<bool>> DeleteRegistrationSetup(int registrationSetupID)
        {
            return await _repository.DeleteRegistrationSetup(registrationSetupID);
        }
    }
}
