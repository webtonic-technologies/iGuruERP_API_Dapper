using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Repository.Interfaces
{
    public interface IRegistrationRepository
    {
        Task<ServiceResponse<string>> AddRegistration(Registration request);
        Task<ServiceResponse<List<Registration>>> GetAllRegistrations(GetAllRequest request);
        Task<ServiceResponse<string>> SendRegistrationMessage(SendRegistrationMessageRequest request);
        Task<ServiceResponse<List<RegistrationSMS>>> GetRegistrationSMSReport();
    }
}
