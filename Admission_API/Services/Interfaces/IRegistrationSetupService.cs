using Admission_API.DTOs.Requests;
using Admission_API.DTOs.Response;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Services.Interfaces
{
    public interface IRegistrationSetupService
    { 
        Task<ServiceResponse<string>> AddUpdateRegistrationSetup(RegistrationSetup request, List<RegistrationOption> options); 
        Task<ServiceResponse<List<RegistrationSetupResponse>>> GetAllRegistrationSetups(GetAllRequest request);  
        Task<ServiceResponse<bool>> DeleteRegistrationSetup(int registrationSetupID);
        Task<ServiceResponse<bool>> FormStatus(int registrationSetupID);
        Task<ServiceResponse<bool>> MandatoryStatus(int registrationSetupID);
    }
}
