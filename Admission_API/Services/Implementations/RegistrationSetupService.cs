using Admission_API.DTOs.Requests;
using Admission_API.DTOs.Response;
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

        public async Task<ServiceResponse<string>> AddUpdateRegistrationSetup(RegistrationSetup request, List<RegistrationOption> options)
        {
            return await _repository.AddUpdateRegistrationSetup(request, options);
        } 

        public async Task<ServiceResponse<List<RegistrationSetupResponse>>> GetAllRegistrationSetups(GetAllRequest request)
        {
            return await _repository.GetAllRegistrationSetups(request);
        } 

        public async Task<ServiceResponse<bool>> DeleteRegistrationSetup(int registrationSetupID)
        {
            return await _repository.DeleteRegistrationSetup(registrationSetupID);
        } 
        public async Task<ServiceResponse<bool>> FormStatus(int registrationSetupID)
        {
            return await _repository.FormStatus(registrationSetupID);
        }
        public async Task<ServiceResponse<bool>> MandatoryStatus(int registrationSetupID)
        {
            return await _repository.MandatoryStatus(registrationSetupID);
        }
    }
}
