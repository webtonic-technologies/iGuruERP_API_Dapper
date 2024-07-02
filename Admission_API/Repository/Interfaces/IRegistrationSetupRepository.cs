using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Repository.Interfaces
{
    public interface IRegistrationSetupRepository
    {
        Task<ServiceResponse<string>> AddUpdateRegistrationSetup(RegistrationSetup request);
        Task<ServiceResponse<List<RegistrationSetup>>> GetAllRegistrationSetups(GetAllRequest request);
        Task<ServiceResponse<bool>> DeleteRegistrationSetup(int registrationSetupID);
    }
}
