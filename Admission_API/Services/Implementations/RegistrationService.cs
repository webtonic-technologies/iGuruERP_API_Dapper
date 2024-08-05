using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using Admission_API.Repository.Interfaces;
using Admission_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Services.Implementations
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _repository;

        public RegistrationService(IRegistrationRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<string>> AddRegistration(Registration request)
        {
            return await _repository.AddRegistration(request);
        }

        public async Task<ServiceResponse<List<Registration>>> GetAllRegistrations(GetAllRequest request)
        {
            return await _repository.GetAllRegistrations(request);
        }

        public async Task<ServiceResponse<string>> SendRegistrationMessage(SendRegistrationMessageRequest request)
        {
            return await _repository.SendRegistrationMessage(request);
        }

        public async Task<ServiceResponse<List<RegistrationSMS>>> GetRegistrationSMSReport()
        {
            return await _repository.GetRegistrationSMSReport();
        }
    }
}
